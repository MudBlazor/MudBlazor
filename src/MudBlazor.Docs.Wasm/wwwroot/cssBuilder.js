window.createStylesheet = function (id) {
    let sheet = document.createElement('style');
    sheet.setAttribute('id', id);
    document.body.appendChild(sheet);
}

window.removeStylesheet = function (id) {
   let sheet = document.getElementById(id);
   document.body.remove(sheet);
}

window.applyDelta = function (sheetId, delta) {
    if(sheetId)
        var sheet = document.getElementById(sheetId);
    else
        return false;
    console.log(sheet);
    console.log(delta);
    delta.forEach(element => {
        if(element.rule == null){
            sheet.deleteRule(element.index);
        }else{
            sheet.insertRule(element.rule, element.index)
        }
    });
    return true;
}

window.apply = function (sheetId, content) {
    if(sheetId)
        var sheet = document.getElementById(sheetId);
    else
        return false;
    sheet.innerHTML = content;
    return true;
}