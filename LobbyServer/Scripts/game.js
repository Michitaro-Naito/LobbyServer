$(function () {
    // SignalR
    $.connection.hub.url = "http://localhost:8080/signalr";

    var game = {
        Log: function (name, body) {
            this.name = name;
            this.body = body;
        },

        Character: function (data) {
            this.name = data.name;
        },

        Room: function (data) {
            this.roomId = data.roomId;
            this.guid = data.guid;
            this.name = data.name;
            this.max = data.max;
            this.interval = data.interval;
        },

        RoomMessage: function (root, data) {
            this.id = data.id;
            this.Created = new Date(data.Created);
            this.callerUserId = data.callerUserId;
            this.mode = data.mode;
            this.fromId = data.fromId;
            this.toId = data.toId;
            this.bodyRows = data.bodyRows;

            this.fmClass = ko.computed(function () {
                return 'mode' + this.mode;
            }, this);
            this.fmCreated = ko.computed(function () {
                var minutes = this.Created.getMinutes();
                if(minutes < 10)
                    minutes = '0' + minutes;
                return this.Created.getHours() + ':' + minutes;
            }, this);

            this.cpFrom = ko.computed(function () {
                if (this.fromId === null)
                    return '';
                for (var n = 0; n < root.actors().length; n++) {
                    var a = root.actors()[n];
                    if (a.id === this.fromId)
                        return a.fmTitleAndName();
                }
                return this.callerUserId;
            }, this);

            this.cpTo = ko.computed(function () {
                if (this.fromId === null)
                    return '';
                var target = '?';
                if (this.mode !== 3) {
                    target = 'All';
                } else {
                    for (var n = 0; n < root.actors().length; n++) {
                        var a = root.actors()[n];
                        if (a.id === this.toId) {
                            target = a.fmTitleAndName();
                            break;
                        }
                    }
                }
                return 'To ' + target;
            }, this);
        },

        Actor: function (root, data) {
            this.root = root;
            this.id = data.id;
            this.title = data.title;
            this.name = data.name;
            this.gender = data.gender;
            this.character = data.character;
            this.role = data.role;
            this.isRoleSure = data.isRoleSure;
            this.isDead = data.isDead;
            this.isRoomMaster = data.isRoomMaster;

            this.fmTitleAndName = ko.computed(function () {
                return this.title + ' ' + this.name;
            }, this);
            this.fmGender = ko.computed(function () {
                var str = '';
                if (this.isRoomMaster)
                    str += '★';
                str += this.gender;
                return str;
            }, this);
            this.fmRole = ko.computed(function () {
                console.info(this.root);
                var role = this.root.FindRole(this.role);
                return role.name;
                /*var str = this.role;
                if (!this.isRoleSure)
                    str += '?';
                return str;*/
            }, this);
            this.fmUser = ko.computed(function () {
                return this.character;
            }, this);
        },

        AppModel: function (culture, pass, hub) {
            var s = this;

            // ----- Variable -----

            s.culture = culture;
            s.pass = pass;
            s.hub = hub;

            s.State = {
                Disconnected: 0000,
                Characters: 1000,
                CreateCharacter: 1001,
                Rooms: 2000,
                Playing: 3000
            };
            s.RoomState = {
                Configuring: 0000,
                Matchmaking: 0001,
                Playing: 0002,
                Ending: 0003,
                Ended: 0004
            };
            s.roomSendModes = ko.observableArray([
                { id: 0, name: 'All' },
                { id: 1, name: 'Wolf' },
                { id: 2, name: 'Ghost' },
                { id: 3, name: 'Private' }
            ]);
            s.state = ko.observable(s.State.Disconnected);
            s.roomState = ko.observable(s.RoomState.Configuring);

            // Characters who Player owns.
            s.characters = ko.observableArray([]);
            s.rooms = ko.observableArray([]);

            s.actors = ko.observableArray([]);
            s.roomSendMode = ko.observable(s.roomSendModes()[0]);
            s.roomSendTo = ko.observable();
            s.roomMessages = ko.observableArray([]);

            s.actorToExecute = ko.observable();
            s.actorToAttack = ko.observable();
            s.actorToFortuneTell = ko.observable();
            s.actorToGuard = ko.observable();
            s.myActorId = ko.observable();
            s.ignoreVoteSubscription = ko.observable(false);
            s.duration = ko.observable(0.0);

            s.logs = ko.observableArray([]);
            s.lastUpdate = new Date();
            s.lastError = ko.observable('');
            s.roles = ko.observableArray([]);

            // ----- Computed -----
            s.cpMyActor = ko.computed(function () {
                for (var n = 0; n < s.actors().length; n++) {
                    var a = s.actors()[n];
                    if (a.id === s.myActorId())
                        return a;
                }
                return null;
            }, this);
            s.cpActorsExceptMe = ko.computed(function () {
                var actors = [];
                for (var n = 0; n < s.actors().length; n++) {
                    var a = s.actors()[n];
                    if (a.id !== s.myActorId())
                        actors.push(a);
                }
                return actors;
            }, this);
            s.fmDuration = ko.computed(function () {
                var totalSeconds = Math.ceil(s.duration());
                var hours = Math.floor(totalSeconds / 3600);
                var minutes = Math.floor(totalSeconds % 3600 / 60);
                if (minutes < 10)
                    minutes = '0' + minutes;
                var seconds = Math.floor(totalSeconds % 60);
                if (seconds < 10)
                    seconds = '0' + seconds;
                var str = '';
                if (hours > 0)
                    str += hours + ':';
                if (hours > 0 || minutes > 0)
                    str += minutes + ':';
                str += seconds;
                return str;
            });



            // ----- Subscription -----

            s.state.subscribe(function (newValue) {
                switch (newValue) {
                    case s.State.Characters:
                        s.Send('/GetCharacters');
                        break;
                    case s.State.CreateCharacter:
                        break;
                    case s.State.Rooms:
                        s.Send('/GetRooms');
                        break;
                    case s.State.Playing:
                        break;
                }
            });

            s.actorToExecute.subscribe(function (newValue) {
                s.Vote();
            });
            s.actorToAttack.subscribe(function (newValue) {
                s.Vote();
            });
            s.actorToFortuneTell.subscribe(function (newValue) {
                s.Vote();
            });
            s.actorToGuard.subscribe(function (newValue) {
                s.Vote();
            });


            // ----- Callback -----

            s.hub.client.addMessage = function (name, body) {
                s.logs.unshift(new Apwei.Game.Log(name, body));
                while (s.logs().length > 100) {
                    s.logs.pop();
                }
            }

            s.hub.client.broughtTo = function (state) {
                s.state(state);
            }

            s.hub.client.gotCharacters = function (characters) {
                s.characters([]);
                for (var n = 0; n < characters.length; n++) {
                    var c = characters[n];
                    s.characters.push(new Apwei.Game.Character(c));
                }
            }

            s.hub.client.gotRooms = function (rooms) {
                s.rooms([]);
                for (var n = 0; n < rooms.length; n++) {
                    var r = rooms[n];
                    s.rooms.push(new Apwei.Game.Room(r));
                }
            }

            s.hub.client.gotError = function (data) {
                s.lastError(data);
                $('#ErrorModal').modal('show');
            }

            s.hub.client.gotRoles = function (roles) {
                s.roles(roles);
                //console.info(s.roles());
            }

            // ----- Callback (in Room) -----

            s.hub.client.gotRoomState = function (newRoomState) {
                $('.modal').modal('hide');
                s.roomState(newRoomState);
            }

            s.hub.client.gotActors = function (actors) {
                s.ignoreVoteSubscription(true);

                // Refresh array
                var newActors = [];
                for (var n = 0; n < actors.length; n++) {
                    newActors.push(new Apwei.Game.Actor(s, actors[n]));
                }
                s.actors(newActors);

                s.ignoreVoteSubscription(false);
            }

            s.hub.client.gotYourActorId = function (id) {
                s.myActorId(id);
            }

            s.hub.client.gotYourSelections = function (data) {
                s.ignoreVoteSubscription(true);

                // Restore selections (if avairable)
                for (var n = 0; n < s.cpActorsExceptMe().length; n++) {
                    var a = s.cpActorsExceptMe()[n];
                    if (a.id === data.executeId) {
                        s.actorToExecute(a);
                        break;
                    }
                }
                for (var n = 0; n < s.cpActorsExceptMe().length; n++) {
                    var a = s.cpActorsExceptMe()[n];
                    if (a.id === data.attackId) {
                        s.actorToAttack(a);
                        break;
                    }
                }
                for (var n = 0; n < s.cpActorsExceptMe().length; n++) {
                    var a = s.cpActorsExceptMe()[n];
                    if (a.id === data.fortuneTellId) {
                        s.actorToFortuneTell(a);
                        break;
                    }
                }
                for (var n = 0; n < s.cpActorsExceptMe().length; n++) {
                    var a = s.cpActorsExceptMe()[n];
                    if (a.id === data.guardId) {
                        s.actorToGuard(a);
                        break;
                    }
                }

                s.ignoreVoteSubscription(false);
            }

            s.hub.client.gotRoomMessages = function (messages) {
                for (var n = 0; n < messages.length; n++) {
                    s.roomMessages.unshift(new Apwei.Game.RoomMessage(s, messages[n]));
                }
            }

            s.hub.client.gotModes = function (modes) {
                s.roomSendModes(modes);
            }

            s.hub.client.gotTimer = function (duration) {
                s.duration(duration);
            }



            // ----- Method -----

            s.Send = function (body) {
                s.hub.server.send(body);
            }

            s.Connect = function () {
                s.hub.connection.start().done(function () {
                    s.hub.server.authenticate(s.culture, s.pass);
                });
                $('#logs').keydown(function (e) {
                    if (event.which == 13) {
                        event.preventDefault();
                        s.hub.server.send($('#logs').val());
                        $('#logs').val('').focus();
                    }
                });
                $('#RoomChat').keydown(function (e) {
                    if (event.which == 13) {
                        event.preventDefault();
                        s.hub.server.roomSend(s.roomSendMode().id, s.roomSendTo().id, $('#RoomChat').val());
                        $('#RoomChat').val('');
                    }
                });
            }

            s.CreateRoom = function () {
                s.Send('/CreateRoom');
            }

            s.RoomConfigure = function () {
                s.hub.server.roomConfigure('Foo', 12, 5);
            }

            s.RoomStart = function () {
                s.hub.server.roomStart();
            }

            s.QuitRoom = function () {
                s.Send('/QuitRoom');
            }

            s.Vote = function () {
                if (s.ignoreVoteSubscription())
                    return;

                s.hub.server.roomVote(
                    s.actorToExecute() ? s.actorToExecute().id : -1,
                    s.actorToAttack() ? s.actorToAttack().id : -1,
                    s.actorToFortuneTell() ? s.actorToFortuneTell().id : -1,
                    s.actorToGuard() ? s.actorToGuard().id : -1);
            }

            // ----- Method (Data Search) -----

            s.FindRole = function (roleId) {
                for (var n = 0; n < s.roles().length; n++) {
                    var role = s.roles()[n];
                    if (role.id === roleId)
                        return role;
                }
                return null;
            }



            s.Update = function () {
                var now = new Date();
                var elapsed = (now - s.lastUpdate) / 1000.0;
                s.lastUpdate = now;

                var duration = s.duration() - elapsed;
                if (duration < 0.0)
                    duration = 0.0;
                s.duration(duration);
            }

            s.Initialize = function () {
                setInterval(s.Update, 100);
            }

            s.Initialize();
        }
    };

    window.Apwei = window.Apwei || {};
    window.Apwei.Game = game;
});