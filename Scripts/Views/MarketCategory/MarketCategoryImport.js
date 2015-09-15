

Ext.onReady(function () {
    var importTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/MarketCategory/ImportMarketCategory',
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
        {//專區Banner
            xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: IMPORT_MARKET,
            msgTarget: 'side',
            buttonText: BROWSE,
            submitValue: true,
            allowBlank: false,
            fileUpload: true
        }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: IMPORT_SUBMIT,
            formBind: true,
            disabled: false,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {

                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                            }
                            else {
                                switch (result.msg) {
                                    case 0://此表內沒有數據或數據有誤,請檢查后再次匯入!
                                        Ext.Msg.alert(INFORMATION, NO_DATA);
                                        break;
                                    case 2:
                                        Ext.Msg.alert(INFORMATION, IMPORT_FAILED);
                                        break;
                                    case 3:
                                        Ext.Msg.alert(INFORMATION, NO_FILE);
                                        break;
                                    default:
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                }

                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (!result.success) {
                                switch (result.msg) {
                                    case 0://此表內沒有數據或數據有誤,請檢查后再次匯入!
                                        Ext.Msg.alert(INFORMATION, NO_DATA);
                                        break;
                                    case 2:
                                        Ext.Msg.alert(INFORMATION, IMPORT_FAILED);
                                        break;
                                    case 3:
                                        Ext.Msg.alert(INFORMATION, NO_FILE);
                                        break;
                                    default:
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            }

                        }
                    });
                }
            }
        }]

    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [importTab],
        autoScroll: true
    });
});