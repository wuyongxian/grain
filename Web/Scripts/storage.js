$(function () {
    // $('#QAccountNumber').val('0040000003');
   
    $('#QAccountNumber').focus(function () { $('#QAccountNumber').val(''); });//储户账号
    $('#QPassword').focus(function () { $('#QPassword').val(''); });//储户密码
});

var dep_result;
//储户查询
function FunDepSelect() {
    var datapara = 'AccountNumber=' + $('#QAccountNumber').val().trim() + '&Password=' + $('#QPassword').val().trim()
    if ($('#QAccountNumber').val().trim().length != 10) {
        showMsg('请输入储户账号');
        return false;
    }
   
    $(".fakeloader").fakeLoader({
        timeToHide: 12000000,
         bgColor: "transparent",
        spinner: "spinner7"
    });
    var url = '/User/Exchange/exchange.ashx?type=getDepositorStorageInfo';
    $.ajax({
        url: url,
        type: 'post',
        data: datapara,
        dataType: 'json',
        async:false,
        success: function (r) {
            $(".fakeloader").fakeCloser();

            $('.depositorInfo').fadeOut();
            $('.depositorStorageInfo').fadeOut();
            dep_result = false;
           // $('.storage').fadeOut();
            if (r.state == false) {
                if (r.msg == '当前的储户不存在存粮记录!') {
                    addDepositor(JSON.parse(r.dep));
                    $('.storage').fadeIn();
                } else {
                    showMsg(r.msg);
                }
                return false;
            }
            //$('.storage').fadeIn();
            var dep = JSON.parse(r.dep);
            var storage = JSON.parse(r.storage);
            addDepositor(dep);

            if (storage.length > 0) {//添加储户存储记录
                addDep_Storage(storage);
            }
            dep_result = true;
            
        }, error: function (r) {
            $(".fakeloader").fakeCloser();

            showMsg('获取储户信息失败 ！');
            dep_result = false;
          
        }
    });
}

function addDepositor(dep) {
    if (dep.length > 0) {//添加储户基本信息
        $('#tabdepositorInfo .trappend').remove();

        var strdep = '<tr class="trappend">';
        strdep += ' <td style="height:30px;">' + dep[0].AccountNumber + '</td>';
        strdep += ' <td>' + dep[0].strName + '</td>';
        strdep += ' <td>' + dep[0].PhoneNO + '</td>';
        var strState = '正常';
        if (dep[0].numState == '0') {
            strState = '禁用';
        }
        strdep += ' <td>' + strState + '</td>';
        strdep += ' <td>' + dep[0].IDCard + '</td>';
        strdep += ' <td>' + dep[0].strAddress + '</td>';
        strdep += '</tr>'
        $('#tabdepositorInfo').append(strdep);
    }
    $('.depositorInfo').fadeIn();
}

function addDep_Storage(storage) {
    $('#tabdepositorStorageInfo .trappend').remove();
    $('#tabdepositorStorageInfo .tr_moneyTotal').remove();
    $('.depositorStorageInfo').fadeIn();
    var moneyTotal = 0;//金额总计
    for (var i = 0; i < storage.length; i++) {
        var strstorage = '';
        strstorage += '<tr class="trappend">';
        if (storage[i].ISVirtual == '1') {
            strstorage += ' <td style="height:25px;">' + storage[i].VarietyName + '<span style="font-size:12px;color:red;">(预)</span></td>';
        } else {
            strstorage += ' <td style="height:25px;">' + storage[i].VarietyName + '</td>';
        }
        strstorage += ' <td>' + storage[i].StorageNumber + '</td>';
        strstorage += ' <td>' + storage[i].StorageDate + '</td>';
        strstorage += ' <td>' + storage[i].Price_ShiChang + '</td>';
        strstorage += ' <td>' + storage[i].TimeName + '</td>';
        strstorage += ' <td>' + storage[i].daycount + '</td>';//存储天数
        strstorage += ' <td>' + storage[i].CurrentRate + '</td>';
        strstorage += ' <td>' + storage[i].strlixi + '</td>';//利息

        moneyTotal += accMul(storage[i].StorageNumber, storage[i].Price_ShiChang) + parseFloat(storage[i].numlixi);

        var SellApplyID = 0;
        if (!isNull(storage[i].SellApplyID)) {
            SellApplyID = storage[i].SellApplyID;
        }

        $('#tabdepositorStorageInfo').append(strstorage);
    }
    moneyTotal = changeTwoDecimal_f(moneyTotal);//折合现金
    var strzhehe = '';
    strzhehe += '<tr class="tr_moneyTotal"> <td colspan="8" style="height:25px; text-align:center;color:#666;font-size:12px;">折合现金合计：￥' + moneyTotal + '</td></tr>';
    $('#tabdepositorStorageInfo').append(strzhehe);


}

