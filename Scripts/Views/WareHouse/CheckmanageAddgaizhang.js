function addFunctiongaizhang(row, store) {
    var editFrms = Ext.create('Ext.form.Panel', {
        id: 'editFrms',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "工作編號",
                name: 'jobnumbers',
                id: 'jobnumbers',
                minValue: 0,
                allowBlank: true
            },
                {
                    xtype: "datefield",
                    editable: false,
                    fieldLabel: '開始時間',
                    id: 'start_time',
                    name: 'start_time',
                    format: 'Y/m/d',
                    value: Tomorrow(-3),
                    allowBlank:false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time").getValue();
                            var end = Ext.getCmp("end_time").getValue();
                            if (start > Tomorrow())
                            {
                                Ext.getCmp("start_time").setValue(Tomorrow(-3));
                                Ext.getCmp("end_time").setValue(Tomorrow(0));
                                Ext.Msg.alert("提示信息", "開始時間不能大於當前時間");
                                return;
                            }
                            if (end <= start) {
                                Ext.getCmp("start_time").setValue(Tomorrow(-3));
                                Ext.getCmp("end_time").setValue(Tomorrow(0));
                                Ext.Msg.alert("提示信息", "結束時間必須要大於開始時間");
                                return;
                            }
                            if (start < Tomorrow(-4)) {
                                Ext.getCmp("start_time").setValue(Tomorrow(-3));
                                Ext.getCmp("end_time").setValue(Tomorrow(0));
                                Ext.Msg.alert("提示信息", "開始時間不允許大於當前時間三天");
                                return;
                            }
                         
                        }
                    }
                },
                {
                    xtype: "datefield",
                    editable: false,
                    fieldLabel: '結束時間',
                    id: 'end_time',
                    name: 'end_time',
                    format: 'Y/m/d',
                    value: Tomorrow(0),
                    allowBlank: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_time").getValue();
                            var end = Ext.getCmp("end_time").getValue();
                            if (end <= start) {
                                Ext.getCmp("start_time").setValue(Tomorrow(-3));
                                Ext.getCmp("end_time").setValue(Tomorrow(0));
                                Ext.Msg.alert("提示信息", "結束時間必須要大於開始時間");
                                return;
                            }
                        }
                    }
                }
        ],
        buttons: [{
            text: "點擊蓋帳",
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();
                Ext.Ajax.request({//蓋帳
                    url: "/WareHouse/UpdateCbjobstaid",
                    params: {
                        jobnumber: Ext.getCmp('jobnumbers').getValue(),
                        starttime: Ext.getCmp('start_time').getValue(),
                        endtime: Ext.getCmp('end_time').getValue()
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                        if (result.success) {
                            if (result.msg >0) {
                                Ext.Msg.alert(INFORMATION, "蓋帳成功,共蓋帳"+result.msg+"個工作編號");
                                CheckmanageStore.load();
                            }
                            else if (result.msg == -1) {
                                Ext.Msg.alert(INFORMATION, "該工作編號目前不能蓋帳!");
                            }
                            else if (result.msg == -2) {
                                Ext.Msg.alert(INFORMATION, "該工作編號不存在或已被刪除!");
                            }
                            else if (result.msg == -3) {
                                Ext.Msg.alert(INFORMATION, "無工作編號可以蓋帳!");
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "蓋帳失敗!");
                        }
                    }
                });

            }
        }]
    });

    var editWins = Ext.create('Ext.window.Window', {
        title: "蓋帳",
        id: 'editWins',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrms],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWins').destroy();
                         CheckmanageStore.load();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {

                    editFrms.getForm().reset();
                }
            }
        }
    });
    editWins.show();
}

function Tomorrow(days) {
    var d;
    d = new Date();
    d.setDate(d.getDate() + days);
    return d;
}