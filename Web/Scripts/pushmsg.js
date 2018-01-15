var apiurl = '';//api地址
var webSiteCode;//网站代码     
var timerNotice;//查询新的未读消息定时器
$(function () {
    getCompanyInfo();//获取网站代码
});

function getCompanyInfo() {
    localStorage.setItem('pushmsg', '');//每次网站启动时，将推送消息清空
    //获取当前站点代码
    $.ajax({
        url: '/Ashx/wbinfo.ashx?type=GetCompanyInfo',
        contentType: 'application/x-www-form-urlencoded',
        type: 'post',
        data: '',
        dataType: 'text',
        success: function (r) {
            if (r != "Error") {
                var jsondata = JSON.parse(r);
                webSiteCode = jsondata[0].webSiteCode;
                apiurl = jsondata[0].pushmsgApiurl;
                localStorage.setItem('webSiteCode', webSiteCode);
                localStorage.setItem('apiurl', apiurl);
                for (var i = localStorage.length - 1 ; i >= 0; i--) {
                    if (localStorage.key(i).indexOf('pushmsg') == 0) {//当缓存中仍有pushmsg时，清除缓存
                        localStorage.removeItem(localStorage.key(i));
                    }
                }
                //GetNotReadNotice();//页面启动时检索依次新消息
                ////检索新信息定时器（每一分钟）s
                //timerNotice = setInterval('GetNotReadNotice()', 60000);//每分钟获取一次消息

            }
        }, error: function (r) { console.log('GetCompanyInfo error'); }
    });
}


function GetNotReadNotice() {
    var apiUri = apiurl + "/api/Notice/GetNotReadNotice?webSiteCode=" + webSiteCode;//webSiteCode-站点代码
    $.ajax({
        url: apiUri,
        type: 'post',
        contentType: 'application/json',
        type: 'get',
        cache: true,
        success: function (data) {
            // console.info(data);
            var jsondata = JSON.parse(data);
            if (jsondata.state == 'error') {
                console.log('response error');
                return false;
            }
            if (jsondata.data.count == 0) {//没有未读消息
                return false;
            }

            //获取当前网点是否已经查看消息
            // var idlist; 
            var noticedata = JSON.parse(data).data.notice;
          
            var idlist = '';
            for (var i = 0; i < noticedata.length; i++) {
                if (i != 0) {
                    idlist = idlist + ',\'' + noticedata[i].ID+'\'';
                } else {
                    idlist = '\'' + noticedata[i].ID + '\'';
                }
            }
            $.ajax({
                url: '/Ashx/wbinfo.ashx?type=GetWBPushMsgState&idlist=' + idlist,
                type: 'post',
                contentType: 'application/json',
                async: true,
                data: '',
                dataType: 'text',
                beforeSend: function (r) { },
                success: function (r) {
                    if (r !="") {//存在该网点未阅读的消息
                       // clearInterval(timerNotice);//清除新消息获取定期器
                        var msglist = localStorage.getItem('msglist');
                        if (msglist == null || msglist == '') {
                            for (var i = 0; i < noticedata.length; i++) {
                                if (noticedata[i].ID == r) {
                                    localStorage.setItem('msglist', JSON.stringify(noticedata[i]));
                                    break;
                                }
                            }
                           
                        }
                    }
                },
                error: function (r) { }
            });

        }
    });


}
