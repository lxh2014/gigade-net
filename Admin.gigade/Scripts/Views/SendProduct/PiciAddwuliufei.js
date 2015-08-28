DeliverStore.load();
Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/SendProduct/HuiruPiciAddwuliufei',
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
            {
                xtype: 'combobox',
                id: 'shipment',
                fieldLabel: '物流商',
                colName: 'shipment',
                queryMode: 'local',
                editable: false,
                store: DeliverStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                value:1
            },
        {
            xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: '檔案',
            msgTarget: 'side',
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
        }

        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue()),
                            shipment: Ext.getCmp('shipment').getValue()
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (parseInt(result.total) > 0) {
                                    Ext.Msg.alert("提示信息", result.msg + ",共匯入數據" + result.total + "條");
                                }
                                else {
                                    Ext.Msg.alert("提示信息", result.msg);
                                }
                             
                              
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert("提示信息", result.msg);
                        }
                    });
                }
            }
        }]

    });
    var hpanel = Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px;background:#87CEEB",
        border: false,
        html: "注意事項：<br/>1.檔案為.xls<br/>2.欄位：付款單號、物流編號、運費、代收金額.<br/>3.當檔案中存在異常時,將會將異常數據匯出,且其它數據不進行操作."
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});