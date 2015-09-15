ImportFunction = function () {
    var ExportFrm = Ext.create('Ext.form.Panel', {
        id: 'ExportFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/InspectionReport/ImportExcel',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'filefield',
                margin: '10 0 10 0',
                name: 'ImportExcelFile',
                id: 'ImportExcelFile',
                fieldLabel: '匯入',
                msgTarget: 'side',
                labelWidth: 70,
                width: 360,
                buttonText: '瀏覽..',
                submitValue: true,
                allowBlank: false,
                fileUpload: true,
                validator:
                    function (value) {
                        var type = value.split('.');
                        if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                            return true;
                        }
                        else {
                            return '上傳文件類型不正確！';
                        }
                    }
            },

             {
                 html: '<font color=red>注意事項：<br/>1.匯入數據時要逐條檢查,會比較耗時.<br/>2.請嚴格按照給定的模板格式匯入,數據不正確的將自動略過.<br/>3.當數據量比較大時,建議分割匯入.</font>' + "<br/><a href='#' onclick='updownmuban()'>點擊下載模板</a>"
                 // bodyStyle: "padding:5px;background:#7DC64C",
                 ,
                 height: 40,
                 border: 0

             }
        ],
        buttons: [{
            text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        waitMsg: '匯入數據中,請稍等......',
                        waitTitle: '提示',
                        timeout: 9000000,
                        params: {
                            ImportExcelFile: Ext.htmlEncode(Ext.getCmp('ImportExcelFile').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 1)
                                {
                                    Ext.Msg.alert("提示", "匯入完成!");
                                    ExportWin.close();
                                    InspectionReportStore.load();
                                }
                                else if (result.msg == 3)
                                {
                                    Ext.Msg.alert("提示", "無匯入數據!");
                                }
                            }
                            else {
                                Ext.Msg.alert("提示", "匯入過程中出錯!");
                            }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert("提示信息", "匯入失敗");
                        }
                    });
                }
            }
        }]
    });

    var ExportWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'ExportWin',
        width: 400,
        height: 250,
        y: 100,
        layout: 'fit',
        items: [ExportFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示", "確認關閉？", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('ExportWin').destroy();

                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ]
    });
    ExportWin.show();
}
function updownmuban() {
    window.open("/InspectionReport/UpdownTemplate");
}