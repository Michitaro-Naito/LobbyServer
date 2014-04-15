$(function () {

    // SignalR
    $.connection.hub.url = "http://localhost:8080/signalr";



    var game = {

        Character: function (data) {
            this.name = data.name;
        },

        RoomConfiguration: function () {
            var s = this;
            s.ModelName = 'RoomConfiguration';
            s.name = ko.observable("Let's play.");
            s.password = ko.observable('');
            s.max = ko.observable(12);
            s.interval = ko.observable(300);
        },

        CreateCharacterData: function(){
            var s = this;
            s.ModelName = 'CreateCharacterData';
            s.name = ko.observable('');
        },

        Room: function (data) {
            var s = this;
            this.roomId = data.roomId;
            this.guid = data.guid;
            this.name = data.name;
            this.max = data.max;
            this.interval = data.interval;
            s.requiresPassword = data.requiresPassword;
            s.aliveActors = data.aliveActors;
            s.alivePlayers = data.alivePlayers;
            s.state = data.state;
            s.clRow = ko.computed(function () {
                if(s.requiresPassword)
                    return 'mode3';
                return '';
            });
            s.fmPlayers = ko.computed(function () {
                if (s.state === 1)
                    return s.aliveActors + '/' + s.max;
                return s.alivePlayers + '/' + s.aliveActors + '/' + s.max;
            });
        },

        LobbyMessage: function(root, data){
            var s = this;
            s.root = root;
            s.Created = new Date(data.Created);
            s.name = data.name;
            s.body = data.body;
            s.fmClass = ko.computed(function () {
                if (s.name === 'test')
                    return 'mode1';
                return 'mode0';
            });
            this.fmCreated = ko.computed(function () {
                var minutes = s.Created.getMinutes();
                if (minutes < 10)
                    minutes = '0' + minutes;
                return s.Created.getHours() + ':' + minutes;
            });
        },

        RoomMessage: function (root, data) {
            var s = this;
            s.root = root;
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
            s.stName = ko.computed(function () {
                var sender = Enumerable.From(s.root.actors())
                    .FirstOrDefault(null, function (a) { return a.id === s.fromId });
                if (sender == null)
                    return '';
                return 'color:' + sender.ColorIdentity.text + '; background:' + sender.ColorIdentity.background + ';';
            });

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
            s.canReport = ko.computed(function () {
                if (s.fromId === null)
                    return false;
                return true;
            });

            s.report = function () {
                root.roomReportMessageId(s.id);
                root.roomReportNote('');
                $('#ReportModal').modal('show');
            }
        },

        Actor: function (root, data) {
            var s = this;
            s.root = root;
            s.id = data.id;
            s.title = data.title;
            s.name = data.name;
            s.gender = data.gender;
            this.character = data.character;
            this.role = data.role;
            this.isRoleSure = data.isRoleSure;
            this.isDead = data.isDead;
            this.isRoomMaster = data.isRoomMaster;
            s.isPresent = data.isPresent;
            s.ColorIdentity = data.ColorIdentity;

            s.IsLocal = ko.computed(function () {
                return s.id === root.myActorId();
            });
            this.fmTitleAndName = ko.computed(function () {
                return this.title + ' ' + this.name;
            }, this);
            this.fmGender = ko.computed(function () {
                var str = '';
                if (s.isRoomMaster)
                    str += '★';
                var gender = Enumerable.From(s.root.genders())
                    .FirstOrDefault(null, function (g) { return g.id === s.gender; });
                str += gender.name;
                return str;
            });
            this.fmRole = ko.computed(function () {
                var role = this.root.FindRole(this.role);
                var str = role.name;
                if (!this.isRoleSure)
                    str += '?';
                return str;
            }, this);
            this.fmUser = ko.computed(function () {
                if (s.character === null)
                    return 'NPC';
                return this.character;
            }, this);
            s.clItem = ko.computed(function () {
                if (s.IsLocal())
                    return 'me';
                return '';
            });
            s.stUser = ko.computed(function () {
                if (s.character === null)
                    return 'color:green;'
                if (s.isPresent)
                    return 'color:#333;';
                return 'color:#CCC;';
            });
            this.cpStyle = ko.computed(function () {
                //if (!this.root)
                //    return;
                var role = this.root.FindRole(this.role);
                return 'background-position: ' + (-24 * role.x) + 'px ' + (-24 * role.y) + 'px;';
            }, this);
            s.stName = ko.computed(function () {
                return 'color:' + s.ColorIdentity.text + '; background:' + s.ColorIdentity.background + ';';
            });
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
            s.Faction = {
                None: 0000,
                Citizen: 0001,
                Werewolf: 0002,
                Fox: 0003
            };
            s.roomSendModes = ko.observableArray([
                { id: 0, name: 'All' },
                { id: 1, name: 'Wolf' },
                { id: 2, name: 'Ghost' },
                { id: 3, name: 'Private' }
            ]);
            s.state = ko.observable(s.State.Disconnected);
            s.roomState = ko.observable(s.RoomState.Configuring);
            s.bootTime = ko.observable(null);

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

            //s.logs = ko.observableArray([]);
            s.lastUpdate = new Date();
            s.lastError = ko.observable('');

            s.roles = ko.observableArray([]);
            s.genders = ko.observableArray([]);
            s.strings = ko.observableArray([]);

            // ----- OnConnected -----
            s.hub.client.gotBootTime = function (time) {
                if (s.bootTime() !== null && s.bootTime() !== time) {
                    alert('サーバが再起動されたようです。ページを再読み込みします...');
                    location.reload();
                }
                s.bootTime(time);
            }

            // ----- Character Creation Scene -----
            s.createCharacterData = ko.observable(new Apwei.Game.CreateCharacterData());
            s.CreateCharacter = function () {
                s.hub.server.createCharacter('CharacterCreation', s.createCharacterData().name());
            }

            // ----- Lobby Scene -----
            s.lobbyMessages = ko.observableArray([]);
            s.hub.client.gotLobbyMessages = function (messages, clear) {
                if (clear)
                    s.lobbyMessages([]);
                for (var n = 0; n < messages.length; n++) {
                    s.lobbyMessages.unshift(new Apwei.Game.LobbyMessage(s, messages[n]));
                }
                while (s.lobbyMessages().length > 50)
                    s.lobbyMessages.pop();
            }
            s.GetRooms = function () {
                s.Send('/GetRooms');
            }
            s.LobbySend = function () {
                var str = $('#LobbyChat').val();
                if (str.length === 0)
                    return;
                s.hub.server.lobbySend(str);
                $('#LobbyChat').val('');
            }
            $('#LobbyChat').keydown(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    s.LobbySend();
                }
            });

            // ----- Room, Join -----
            s.roomGoingToJoin = ko.observable();
            s.roomPasswordGoingToJoin = ko.observable();

            // ----- Room, Configure -----
            s.roomConfigurationsToSet = ko.observable(new Apwei.Game.RoomConfiguration());
            s.validationErrors = {
                RoomConfiguration: ko.observableArray([]),
                CharacterCreation: ko.observableArray([])
            };

            // ----- Room Scene -----
            // Variable
            s.roomConfigurations = ko.observable();
            s.roomReportMessageId = ko.observable();
            s.roomReportNote = ko.observable('');
            s.factionWon = ko.observable();
            // Computed
            s.cpMyActor = ko.computed(function () {
                return Enumerable.From(s.actors()).FirstOrDefault(null, function (a) { return a.id === s.myActorId(); });
            });
            s.cpAliveActors = ko.computed(function () {
                return Enumerable.From(s.actors()).Where(function (a) { return !a.isDead; }).ToArray();
            });
            s.cpDeadActors = ko.computed(function () {
                return Enumerable.From(s.actors()).Where(function (a) { return a.isDead; }).ToArray();
            });
            s.cpAliveActorsExceptMe = ko.computed(function () {
                return Enumerable.From(s.actors()).Where(function (a) { return !a.isDead && a.id !== s.myActorId(); }).ToArray();
            });
            s.IsLocalAlive = ko.computed(function () {
                var me = s.cpMyActor();
                if (me === null)
                    return false;
                if (me.isDead)
                    return false;
                return true;
            });
            s.IsExecuteVisible = ko.computed(function () {
                var me = s.cpMyActor();
                if (me === null)
                    return false;
                if (me.isDead)
                    return false;
                return true;
            });
            s.IsAttackVisible = ko.computed(function () {
                var me = s.cpMyActor();
                if (me === null)
                    return false;
                if (me.isDead)
                    return false;
                return me.role === 2000;    // IsWerewolf?
            });
            s.IsFortuneTellVisible = ko.computed(function () {
                var me = s.cpMyActor();
                if (me === null)
                    return false;
                if (me.isDead)
                    return false;
                return me.role === 1001;    // IsFortuneTeller?
            });
            s.IsGuardVisible = ko.computed(function () {
                var me = s.cpMyActor();
                if (me === null)
                    return false;
                if (me.isDead)
                    return false;
                return me.role === 1003;    // IsHunter?
            });
            // Computed (Class)
            s.clSendMessage = ko.computed(function () {
                return 'box mode' + s.roomSendMode().id;
            });
            s.clFactionWon = ko.computed(function () {
                return 'box FactionWon Faction' + s.factionWon();
            });
            // Callback
            s.hub.client.gotRoomConfigurations = function (data) {
                s.roomConfigurations(data);
            }
            s.hub.client.gotModes = function (modes) {
                var mode = s.roomSendMode();
                s.roomSendModes(modes);
                if (mode !== undefined) {
                    var restore = Enumerable.From(s.roomSendModes()).FirstOrDefault(null, function (m) { return m.id === mode.id; });
                    if (restore !== null) {
                        s.roomSendMode(restore);
                    }
                    else
                        // Could not restore. Clear input (to prevent sending accidentally.)
                        $('#RoomChat').val('');
                }
            }
            s.hub.client.gotActors = function (actors) {
                s.ignoreVoteSubscription(true);

                var to = s.roomSendTo();

                // Refresh array
                var newActors = [];
                for (var n = 0; n < actors.length; n++) {
                    newActors.push(new Apwei.Game.Actor(s, actors[n]));
                }
                s.actors(newActors);

                // Restore PrivateMessageTarget
                if (to !== undefined)
                    s.roomSendTo(Enumerable.From(s.actors()).FirstOrDefault(undefined, function (a) { return a.id === to.id; }));

                s.ignoreVoteSubscription(false);
            }
            s.hub.client.gotFactionWon = function (factionWon) {
                console.info('FactionWon: ' + factionWon);
                s.factionWon(factionWon);
            }
            // Method
            s.roomReport = function () {
                s.hub.server.roomReportMessage(s.roomReportMessageId(), s.roomReportNote());
            }
            s.RoomSend = function () {
                var str = $('#RoomChat').val();
                if (str.length === 0)
                    return;
                s.hub.server.roomSend(s.roomSendMode().id, s.roomSendTo().id, str);
                $('#RoomChat').val('');
            }
            // Event
            $('#RoomChat').keydown(function (event) {
                if (event.which == 13) {
                    event.preventDefault();
                    s.RoomSend();
                }
            });



            // ----- Computed -----
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

            //s.Text = function (key) { return 'foo'+key;}
            s.Text = function (key) {
                return ko.computed({
                    read: function () {
                        var entry = Enumerable.From(s.strings())
                            .FirstOrDefault(null, function (e) { return e.Key === key; });
                        if (entry === null)
                            return key;
                        return entry.Value;
                    }
                });
            }


            // ----- Subscription -----

            s.state.subscribe(function (newValue) {
                switch (newValue) {
                    case s.State.Characters:
                        s.Send('/GetCharacters');
                        break;
                    case s.State.CreateCharacter:
                        break;
                    case s.State.Rooms:
                        //s.Send('/GetRooms');
                        s.GetRooms();
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



            // ----- Callback (Lifetime) -----
            s.hub.connection.stateChanged(function (data) {
                console.info(data);
                switch (data.newState) {
                    case 1:
                        // Connected. Authenticates first.
                        s.hub.server.authenticate(s.culture, s.pass);
                        break;

                    case 4:
                        // Disconnected. Goes to DisconnectedScene.
                        s.state(s.State.Disconnected);
                        break;
                }
            });

            s.hub.client.gotDisconnectionRequest = function () {
                s.Disconnect();
            }


            // ----- Callback -----

            s.hub.client.addMessage = function (name, body) {
                console.info(name + ',' + body);
            }

            s.hub.client.broughtTo = function (state) {
                console.info('broughtTo');
                console.info(state);
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
            }

            s.hub.client.gotGenders = function (genders) {
                s.genders(genders);
            }

            s.hub.client.gotStrings = function (strings) {
                s.strings(strings);
                console.info(s.strings());
            }

            s.hub.client.gotValidationErrors = function (model, errors) {
                console.info(model);
                console.info(errors);
                s.validationErrors[model](errors);
            }

            // ----- Callback (in Room) -----

            s.hub.client.gotRoomState = function (newRoomState) {
                $('.modal').modal('hide');
                s.roomState(newRoomState);
            }

            s.hub.client.gotYourActorId = function (id) {
                s.myActorId(id);
            }

            s.hub.client.gotYourSelections = function (data) {
                s.ignoreVoteSubscription(true);

                // Restore selections (if avairable)
                var a = Enumerable.From(s.cpAliveActorsExceptMe()).FirstOrDefault(null, function (a) { return a.id === data.executeId });
                s.actorToExecute(a);
                var a = Enumerable.From(s.cpAliveActorsExceptMe()).FirstOrDefault(null, function (a) { return a.id === data.attackId });
                s.actorToAttack(a);
                var a = Enumerable.From(s.cpAliveActorsExceptMe()).FirstOrDefault(null, function (a) { return a.id === data.fortuneTellId });
                s.actorToFortuneTell(a);
                var a = Enumerable.From(s.cpAliveActorsExceptMe()).FirstOrDefault(null, function (a) { return a.id === data.guardId });
                s.actorToGuard(a);

                s.ignoreVoteSubscription(false);
            }

            s.hub.client.gotRoomMessages = function (messages, clear) {
                if (clear)
                    s.roomMessages([]);
                for (var n = 0; n < messages.length; n++) {
                    s.roomMessages.unshift(new Apwei.Game.RoomMessage(s, messages[n]));
                }
            }

            s.hub.client.gotTimer = function (duration) {
                s.duration(duration);
            }



            // ----- Method -----

            s.Send = function (body) {
                s.hub.server.send(body);
            }

            s.Connect = function () {
                s.hub.connection.start();
            }

            s.Disconnect = function () {
                s.hub.connection.stop();
            }

            s.CreateRoom = function () {
                s.Send('/CreateRoom');
            }

            s.OpenRoomModal = function (/*roomId*/room) {
                console.info(room);
                s.roomGoingToJoin(room);
                /*s.roomIdGoingToJoin(room.roomId);*/
                $('#RoomModal').modal('show');
            }

            s.JoinRoom = function () {
                //s.hub.server.joinRoom(s.roomIdGoingToJoin(), s.roomPasswordGoingToJoin());
                s.hub.server.joinRoom(s.roomGoingToJoin().roomId, s.roomPasswordGoingToJoin());
            }

            s.RoomConfigure = function () {
                s.hub.server.roomConfigure({
                    name: s.roomConfigurationsToSet().name(),
                    password: s.roomConfigurationsToSet().password(),
                    max: s.roomConfigurationsToSet().max(),
                    interval: s.roomConfigurationsToSet().interval(),
                    ModelName: s.roomConfigurationsToSet().ModelName
                });
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
                $('#Game').show();
                $('#logs').keydown(function (event) {
                    if (event.which == 13) {
                        event.preventDefault();
                        s.hub.server.send($('#logs').val());
                        $('#logs').val('').focus();
                    }
                });
            }

            s.Initialize();
        }
    };

    window.Apwei = window.Apwei || {};
    window.Apwei.Game = game;
});