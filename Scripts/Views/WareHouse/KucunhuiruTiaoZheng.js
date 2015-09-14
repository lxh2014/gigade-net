Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/WareHouse/InsertkucunMessage',
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
            xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: '匯入庫存信息對照表',
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
        ,
        {
            xtype: 'displayfield',hidden:true,
            value: '<a href="../../Template/Boiler/庫存信息.xls">' + "下載範例" + '</a>'
        }
        ],
        buttonAlign: 'right',
        buttons: [
           {text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                var files = result.msg;//文件信息
                                var iialgtotal = result.iialgtotal;//匯入數據條數
                                var iinvdtotal = result.iinvdtotal;//更改數據數
                                if (parseInt(iialgtotal) + parseInt(iinvdtotal) > 0) {
                                    Ext.Msg.alert("提示信息", files + ",共匯入iialg表數據" + iialgtotal + "條,共更改iinvd表數據" + iinvdtotal + "條");
                                }
                                else {
                                    Ext.Msg.alert("提示信息", files);
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
           }
        ]
 
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        border: false,
        html: ""
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});