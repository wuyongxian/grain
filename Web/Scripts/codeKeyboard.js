var ISCodekeyboard;//是否启用密码键盘
$(function () {
    ISCodekeyboard = JSON.parse(localStorage.getItem("WBAuthority")).ISCodekeyboard;
});

//检测是否是密码键盘输入
document.onkeydown = function (event) {
    var e = event || window.event || arguments.callee.caller.arguments[0];
    if (ISCodekeyboard) {
        if (e.keyCode > 47 && e.keyCode < 58) {
            if (document.activeElement.id != 'QPassword') {
                $('#QPassword').val('');
                return false;
            }
        }
        if (e.keyCode == 13 && $('#QSelect').attr('disabled') == false) {
            FunDepSelect();
        }
    }
    if (e.keyCode == 13) {//确认按键
    }
};