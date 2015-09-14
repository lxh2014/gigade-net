Ext.require([
    'Ext.form.*'
]);
Ext.onReady(function () {
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：匯出美安訂單表</div>',
                frame: false,
                border: false
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'datefield',
                        fieldLabel: "訂單訂購日期",
                        id: 'start_time',
                        editable: false
                        //,
                        //listeners: {
                        //    select: function (a, b, c) {
                        //        //var start = Ext.getCmp("start_time");
                        //        //var end = Ext.getCmp("end_time");
                        //        //var s_date = new Date(start.getValue());
                        //        //if (end.getValue() < start.getValue()) {
                        //        //    Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                        //        //    start.setValue(null);
                        //        //}
                        //    }
                        //}
                    },
                    { xtype: 'displayfield', margin: '0 5 0 5', value: '~', },
                    {
                        xtype: 'datefield',
                        id: 'end_time',
                        editable: false,
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert("提示", "開始時間不能大於結束時間！");
                                    end.setValue(null);
                                }
                            }
                        }
                    },
                ]
            },
            {
                xtype: 'button',
                text: "確定匯出",
                id: 'btnQuery',
                buttonAlign: 'center',
                handler: ExportCountBook
            }
        ],
        renderTo: Ext.getBody()
    });
    function ExportCountBook() {
        window.open("/MarketCategory/ExcelMarketOrder?starttime=" + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s')) + "&endtime=" + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d 23:59:59')));
    }
});
