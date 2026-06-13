"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalrServer")
    .build();

connection.on("LoadAllItems", function () {
    location.reload();
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
