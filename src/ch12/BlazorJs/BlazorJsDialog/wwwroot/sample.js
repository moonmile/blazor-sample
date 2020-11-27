window.jsFunctions = {
    alert: function (msg) {
        alert(msg);
    },
    confirm: function (msg) {
        return confirm(msg) == false ? 0 : 1;
    },
}
