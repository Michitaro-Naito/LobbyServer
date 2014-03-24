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

        RoomMessage: function (data) {
            this.body = data.body;
        },

        Actor: function (data) {
            this.id = data.id;
            this.title = data.title;
            this.name = data.name;
            this.gender = data.gender;
            this.character = data.character;
            this.role = data.role;
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
                return this.role;
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
            s.roomSendModes = [
                { id: 0, name: 'All' },
                { id: 1, name: 'Wolf' },
                { id: 2, name: 'Ghost' },
                { id: 3, name: 'Private' }
            ];
            s.state = ko.observable(s.State.Disconnected);
            s.roomState = ko.observable(s.RoomState.Configuring);

            // Characters who Player owns.
            s.characters = ko.observableArray([]);
            s.rooms = ko.observableArray([]);

            s.actors = ko.observableArray([]);
            s.roomSendMode = ko.observable(s.roomSendModes[0]);
            s.roomSendTo = ko.observable();
            s.roomMessages = ko.observableArray([]);

            s.logs = ko.observableArray([]);



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


            // ----- Callback -----

            s.hub.client.addMessage = function (name, body) {
                s.logs.unshift(new Apwei.Game.Log(name, body));
                while (s.logs().length > 100) {
                    s.logs.pop();
                }
            }

            s.hub.client.broughtTo = function (state) {
                console.info('brought to ' + state);
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

            // ----- Callback (in Room) -----

            s.hub.client.gotRoomState = function (newRoomState) {
                $('#myModal').modal('hide');
                s.roomState(newRoomState);
            }

            s.hub.client.gotActors = function (actors) {
                s.actors([]);
                for (var n = 0; n < actors.length; n++) {
                    s.actors.push(new Apwei.Game.Actor(actors[n]));
                }
            }

            s.hub.client.gotRoomMessages = function (messages) {
                for (var n = 0; n < messages.length; n++) {
                    s.roomMessages.unshift(new Apwei.Game.RoomMessage(messages[n]));
                }
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
        }
    };

    /*var model = new AppModel('culture', 'pass', $.connection.myHub);
    ko.applyBindings(model, document.getElementById('Game'));
    model.Connect();*/
    window.Apwei = window.Apwei || {};
    window.Apwei.Game = game;
});