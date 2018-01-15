//表格鼠标悬停换色
function change_colorOver(e) {
    var oldColor = e.style.backgroundColor;
    document.getElementById("colorName").value = oldColor;
    //e.style.backgroundColor = "#e1d1c1";
    e.style.backgroundColor = "#e1d1c1";
}
function change_colorOut(e) {
    e.style.backgroundColor = document.getElementById("colorName").value;
}

$(function () {
    $('#spanHelp').toggle(function () {
        $('#divHelp').slideDown('normal');
    }, function () {
        $('#divHelp').slideUp('normal');
    });

    $(".pageEidt .imgclose").mouseover(function () {
        $(".pageEidt .imgclose").attr('src', '/images/winClose2.png');
    })
    $(".pageEidt .imgclose").mouseout(function () {
        $(".pageEidt .imgclose").attr('src', '/images/winClose.png');
    })

    $(".datadetail .imgclose").mouseover(function () {
        $(".datadetail .imgclose").attr('src', '/images/winClose2.png');
    })
    $(".datadetail .imgclose").mouseout(function () {
        $(".datadetail .imgclose").attr('src', '/images/winClose.png');
    })

});

//初始化select标签
function InitSelect(strUrl, strName,strAlert) {
    $.ajax({
        url: strUrl,
        type: 'post',
        data: '',
        dataType: 'json',
        success: function (r) {
            for (var i = 0; i < r.length; i++) {
                $('select[name=' + strName + ']').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
            }
        }, error: function (r) {
            showMsg(strAlert);
        }
    });
}


//初始化select标签
function ShowSelect(strUrl, strName,strMessage) {
    $.ajax({
        url: strUrl,
        type: 'post',
        data: '',
        dataType: 'json',
        success: function (r) {
            for (var i = 0; i < r.length; i++) {
                $('select[name=' + strName + ']').append("<option value='" + r[i].ID + "'>" + r[i].strName + "</option>");
            }
        }, error: function (r) {
            showMsg(''+strMessage+'');
        }
    });
}






function SingleDataAdd(url, data) {
    var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
    showConfirm(msg, function (obj) {
        if (obj == 'yes') {
            
            $.ajax({
                url: url,
                type: 'post',
                data: data,
                dataType: 'text',
                success: function (r) {
                    if (r == "OK") {
                        showMsg('添加数据成功 ！');
                        CloseFrm();
                        location.reload();
                    } else if (r == "1") {
                        showMsg('已存在相同的类型名称，请修改后添加 ！');
                    }
                }, error: function (r) {
                    showMsg('添加数据失败 ！');
                }
            });
        } else {
            //console.log('你点击了取消！');
        }
    });
}

function SingleDataUpdate(url,data) {
    var msg = '您确认已经仔细检查输入信息，并继续操作吗？';
    showConfirm(msg, function (obj) {
        if (obj == 'yes') {
            
            $.ajax({
                url: url,
                type: 'post',
                data: data,
                dataType: 'text',
                success: function (r) {
                    if (r == "OK") {
                        showMsg('更新数据成功 ！');
                        CloseFrm();
                        location.reload();
                    } else {
                        showMsg('更新数据失败 ！');
                    }
                }, error: function (r) {
                    showMsg('更新数据失败 ！');
                }
            });
        } else {
            //console.log('你点击了取消！');
        }
    });
}


function SingleDataDelete(url) {
    var msg = '您确认要删除这条数据吗？';
    showConfirm(msg, function (obj) {
        if (obj == 'yes') {
            
            $.ajax({
                url: url,
                type: 'post',
                data: '',
                dataType: 'text',
                success: function (r) {
                    if (r == "Exit") {
                        showMsg('该信息已经在系统中使用，无法删除！', {
                            'type': 'warning',
                            'title': 'warning'
                        });
                    }
                    else if (r == "OK") {
                        showMsg('删除数据成功！');
                        CloseFrm();
                        location.reload();
                    } else {
                        showMsg('删除数据失败 ！');
                    }
                }, error: function (r) {
                    showMsg('删除数据失败 ！');
                }
            });
        } else {
            //console.log('你点击了取消！');
        }

    });

}

function ShowFrm(wbid) {
    var width = $('#divfrm').width();
    var height = $('#divfrm').height();
    //var top = (document.body.scrollHeight - height) / 2;
    //var left = (document.body.scrollWidth - width) / 2;
    var top = (document.documentElement.clientHeight - height) / 2;
    var left = (document.documentElement.clientWidth - width) / 2;
    $('#divfrm').css('position', 'fixed');
    $('#divfrm').css('top', top).css('left', left);
    $('#divfrm').fadeIn("normal");
    $('#WBID').val(wbid);
    if (wbid == "0") {//新增网点
        ShowFrm_Add(wbid);
    }
    else { //编辑网点
        ShowFrm_Update(wbid);
    }
};

function CloseFrm() {
    $('#divfrm').fadeOut("normal");
};


