PintListFunction = function () {
 var htmljosn = '暫無數據';
    Ext.onReady(function () {
       
        var frmLotPrint = Ext.create('Ext.form.Panel', {
            id: 'frmLotPrint',
            margin: '5 0 0 0',
            layout: 'anchor',
            border: false,
            items: [
                //gdOrderDetail
                {
                    xtype: 'button',
                    id: 'buttonPrint',
                    text: '列印',
                    margin: '10 0 10 10',
                    handler: Listprint
                },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    id:'timebar',
                    margin: '10px 0px 10px 0px',
                    items: [
                        {
                            xtype: 'datetimefield',
                            id: 'time_start',
                            fieldLabel: '轉單時間',
                            name: 'time_start',
                            margin: '0 5px 0 5px',
                            editable: false,
                            value: Tomorrow(1 - new Date().getDate())
                        },
                        {
                            xtype: 'displayfield',
                            value: '~'
                        },
                        {
                            xtype: 'datefield',
                            id: 'time_end',
                            name: 'time_end',
                            margin: '0 5px',
                            editable: false,
                            value: Tomorrow(0)
                        },
                          {
                              xtype: 'button',
                              margin: '0 10 0 10',
                              iconCls: 'icon-search',
                              text: "送出",
                              handler: ListQuery
                          }
                    ]
                }
            ]
        });
        var FrmHtmlGrid = Ext.create('Ext.panel.Panel', {
            id: 'frmhtml',
            html: htmljosn,
            renderTo: Ext.getBody()
        });
       
        var lotDeliverDetailPrintWin = Ext.create('Ext.window.Window', {
            title: "列印定單明細",
            id: 'lotDeliverDetailPrintWin',
            iconCls: 'icon-user-edit',
            width: 800,
            height: 680,
            layout: 'anchor',
            items: [
                frmLotPrint, FrmHtmlGrid

                //, {
                //    xtype: 'button',
                //    id: 'buttonPrint',
                //    text: '列印',
                //    handler: print
                //}
                //,
                //ChangeCustomerInfo
            ],
            closeAction: 'destroy',
            modal: true,
            // resizable: false,
            constrain: true,
            labelWidth: 60,
            //bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false,
            tools: [
                {
                    type: 'close',
                    qtip: "關閉",
                    handler: function (event, toolEl, panel) {
                        Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                            if (btn == "yes") {
                                Ext.getCmp('lotDeliverDetailPrintWin').destroy();
                            } else {
                                return false;
                            }
                        });
                    }
                }
            ]
            ,
            listeners: {
                'show': function () {
                    Ext.getCmp('buttonPrint').hide();
                }
            }
        });
        lotDeliverDetailPrintWin.show();
    });
}

//function LoadData(order_id) {
//    Ext.Ajax.request({
//        url: "/SendProduct/GetOrderDeliverDetail",
//        method: 'post',
//        params: {
//            oid: order_id,
//            p_mode: 1
//        },
//        success: function (response) {
//            var result = Ext.JSON.decode(response.responseText);
//            if (result.success) {
//                Ext.getCmp("pnlPrint").update("<form>" + result.msg + "</form>")
//            } else {
//                alert('加載出錯');
//            }
//        },
//        failure: function () {
//            Ext.Msg.alert('系統錯誤！');
//        }
//    });
//}

ListQuery= function () {
    Ext.getBody().mask("请稍等......", "x-mask-loading");
    Ext.Ajax.request({
        url: '/SendProduct/GetOrderDeliverDetailList',
        method: 'post',
        params: {
            start_time: Ext.getCmp('time_start').getValue(),
            end_time: Ext.getCmp('time_end').getValue()
        },
        success: function (response) {
            Ext.getBody().unmask();
            var result = Ext.decode(response.responseText);
            //alert(result.msg);
            if (result.success) {
              
                //Ext.getCmp('timebar').hide();
                Ext.getCmp('buttonPrint').show();
                htmljosn = result.msg;
                Ext.getCmp('frmhtml').update("<form id='abc'><div style='overflow:auto;width:750px;height:550px;'>" + htmljosn + "</div></form>");
            }
            else {
                alert('系統錯誤');
            }
        },
        failure: function () {
            Ext.getBody().unmask();
            Ext.Msg.alert('加載出錯！');
        }
    });

};
function Listprint() {

    bdhtml = Ext.getCmp('frmhtml').html;////window.document.body.innerHTML;//Ext.getCmp("frmhtml").htmljosn;//
    var sd="<form id='abc'><div style='overflow:auto;width:750px;height:550px;'>";
    startprint = ("<form id='abc'><div style='overflow:auto;width:750px;height:550px;'>");
    endprint = ("</form>");
    printhtml = bdhtml.substr(bdhtml.indexOf(startprint) + sd.length);//39
    printhtml = printhtml.substring(0, printhtml.indexOf(endprint));
    // window.document.body.innerHTML = printhtml;
    ////window.document.body.innerHTML = bdhtml;
    //window.print();
    // javascript: print();
    //window.location.reload(); 

  //  var pw = window.open('', '');
    window.document.write('<html>');
    window.document.write('<head>');
    window.document.write('<title></title>');
    window.document.write('</head>');
    window.document.write('<body><from><div>');
    window.document.write(printhtml);
    window.document.write('</div><from></body>');
    window.document.write('</html>');
    //pw.document.close();
    //setTimeout(function () {
    window.print();
    //}, 500);
    //return false;
    window.location.reload();

}
    function Tomorrow(days) {
        var d;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate() + days;                          // 获取日。
        return (new Date(s));                                 // 返回日期。
    }