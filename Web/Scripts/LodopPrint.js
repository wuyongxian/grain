var p_lleft = 10; var p_ltop = 30; var p_lwidth = 640; var p_lheight = 730;
$(function () {
    $('body').append('<div id="divPrint" style="display:none"></div>');
    $('body').append('<div id="divPrintPaper" style="display:none"></div>');
    //$('body').append('<div id="divPrint"></div>');
    //$('body').append('<div id="divPrintPaper"></div>');
    $.ajax({
        url: '/Ashx/wbinfo.ashx?type=GetPrintSetting_Dep',
        type: 'post',
        data: '',
        dataType: 'json',
        success: function (r) {
            p_lwidth = r[0].Width;
            p_lheight = r[0].Height;
            p_lleft = r[0].DriftRateX;
            p_ltop = r[0].DriftRateY;
        }, error: function (r) {
            showMsg('加载打印坐标时出现错误 ！');
        }
    });
});
function CreateOneFormPage() {
    LODOP = getLodop();
    LODOP.PRINT_INIT("存折打印");
    LODOP.SET_PRINT_STYLE("FontSize", 18);
    LODOP.SET_PRINT_STYLE("Bold", 1);
    LODOP.ADD_PRINT_TEXT(0, 0, 0, 0, "打印页面部分内容");
    LODOP.ADD_PRINT_HTM(p_ltop, p_lleft, p_lwidth, p_lheight, $('#divPrint').html());
};

//小票打印
function CreatePage() {
    LODOP = getLodop();
    LODOP.PRINT_INIT("小票打印");
    LODOP.SET_PRINT_STYLE("FontSize", 12);
    LODOP.SET_PRINT_STYLE("Bold", 1);
    LODOP.ADD_PRINT_TEXT(0, 0, 0, 0, "打印页面部分内容");
    LODOP.ADD_PRINT_HTM(20, 60, 800, 400,$('#divPrintPaper').html());
};