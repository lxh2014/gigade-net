ImportFunction = function (row, store) {
    var ExportFrm = Ext.create('Ext.form.Panel', {
        id: 'ExportFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/EdmNew/ImportExcel',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                id: 'group_id',
                name: 'group_id',
                fieldLabel: '編號',
                hidden: true,
            },
            {
                xtype: 'filefield',
                margin: '10 0 10 0',
                name: 'ImportExcel',
                id: 'ImportExcel',
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
                xtype: 'displayfield',
                html: "<a href='#' onclick='downLoadTemplate()'>點擊下載模板</a>",
                width: 490,
                margin:'0 0 0 78',

            },
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
                            ImportExcel: Ext.htmlEncode(Ext.getCmp('ImportExcel').getValue()),
                            group_id: Ext.getCmp('group_id').getValue(),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.wrongCount != 0) {
                                    var zz = result.totalCount - result.wrongCount;
                                    Ext.Msg.alert("提示信息", "共" + result.totalCount + "條數據<br/>匯入成功" + zz + "條<br/>匯入失敗" + result.wrongCount + "條<br/><a href='#' onclick='DownWrongList()'>點擊下載未能匯入數據</a>");
                                }
                                else {
                                    Ext.Msg.alert("提示信息","全部成功匯入！");
                                }
                                ExportWin.close();
                                store.load();
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
        title:'信箱名單匯入',
        id: 'ExportWin',
        width: 443,
        height: 150,
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
        ],
        listeners: {
            'show': function () {
                if (row) {
                    Ext.getCmp('group_id').setValue(row.data.group_id);
                    //editFrm.getForm().loadRecord(row);
                    //Ext.getCmp('group_id').show(true);
                    //Ext.getCmp('s_group_createdate').show(true);
                    //Ext.getCmp('s_group_updatedate').show(true);
                }
            }
        },
    });
    ExportWin.show();
}
function downLoadTemplate() {
    window.open("/EdmNew/ExportTemplateExcel");
}

function DownWrongList()
{
    window.open("/EdmNew/DownWrongList");
}