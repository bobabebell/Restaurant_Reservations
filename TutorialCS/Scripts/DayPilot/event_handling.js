function create(start, end, resource) {
    createModal().showUrl('New.aspx?start=' + start + "&end=" + end + "&table=" + resource);
}

function edit(e) {
    createModal().showUrl('Edit.aspx?id=' + e.value());
}

function createModal() {
    var modal = new DayPilot.Modal();
    modal.top = 60;
    modal.width = 300;
    modal.opacity = 50;
    modal.border = "10px solid #d0d0d0";
    modal.closed = function () {
        if (this.result && this.result.refresh) {
            dp.commandCallBack("refresh", { message: this.result.message });
        }
        dp.clearSelection();
    };

    modal.setHeight = function (height) {
        modal.height = height;
        return modal;
    };

    modal.height = 230;
    modal.zIndex = 100;

    return modal;
}

function afterRender() {
    // update filter
    $("#filter").html('');
    if (!dp.clientState.filter) {
        return;
    }
    for (prop in dp.clientState.filter) {
        var f = dp.clientState.filter[prop];
        var span = document.createElement("span");
        span.innerHTML = f.name + ": " + f.value + " X";
        span.className = "filter";
        $(span).data("filter", prop);
        $(span).click(function () {
            delete dp.clientState.filter[$(this).data("filter")];
            dp.commandCallBack("refresh");
        });
        $("#filter").append(span);
    }
    if (dp.clientState.filter) {
        var span = document.createElement("span");
        span.innerHTML = "Clear All";
        span.className = "filter all";
        $(span).click(function () {
            dp.commandCallBack("clear");
        });
        $("#filter").append(span);
    }
}

function seats(count) {
    if (!dp.clientState.filter) {
        dp.clientState.filter = {};
    }
    var seats = dp.clientState.filter.seats = {};
    seats.name = "People";
    seats.value = count;
    seats.count = count;

    dp.commandCallBack("refresh");
}