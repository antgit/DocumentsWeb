function daysInMonth(month, year) {
    var d = new Date(year, month + 1, 0);
    return d.getDate(); // last day in January
}

function GetEndOfMonth() {
    var date = new Date();
    date.setDate(daysInMonth(date.getMonth(), date.getYear()));
    return date;
}

function GetEndOfNextMonth() {
    var date = new Date();
    date.setMonth(date.getMonth() + 1);
    date.setDate(daysInMonth(date.getMonth(), date.getYear()));
    return date;
}

function GetEndOfWeek() {
    var date = new Date();
    date.setDate(date.getDate() + 7 - date.getDay());
    return date;
}

function GetEndOfNextWeek() {
    var date = new Date();
    date.setDate(date.getDate() + 14 - date.getDay());
    return date;
}

function GetEndOfTwoWeeks() {
    var date = new Date();
    date.setDate(date.getDate() + 21 - date.getDay());
    return date;
}

function GetEndOfThreeWeeks() {
    var date = new Date();
    date.setDate(date.getDate() + 28 - date.getDay());
    return date;
}

function GetTomorrow() {
    var date = new Date();
    date.setDate(date.getDate() + 1);
    return date;
}

function GetDayAfterTomorrow() {
    var date = new Date();
    date.setDate(date.getDate() + 2);
    return date;
}