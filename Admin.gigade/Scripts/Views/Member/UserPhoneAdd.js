//地址Model
Ext.define('gigade.City', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "middle", type: "string" },
        { name: "middlecode", type: "string"}]
});

//郵編Model
Ext.define('gigade.Zip', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "zipcode", type: "string" },
        { name: "small", type: "string"}]
});


//會員地址Store
var CityStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.City',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Member/QueryCity",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});


//會員郵編Store
var ZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    remoteSort: false,
    autoLoad: false,
    model: 'gigade.Zip',
    proxy: {
        type: 'ajax',
        url: "/Member/QueryZip",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});



ZipStore.on('beforeload', function () {
    Ext.apply(ZipStore.proxy.extraParams,
    {
        topValue: Ext.getCmp("cob_ccity").getValue(),
        topText: Ext.getCmp("cob_ccity").rawValue
    });
});


////生日年
//Ext.define('gigade.Year', {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "year", type: "int"}]
//});
//var yearStore = Ext.create('Ext.data.Store', {
//    autoDestroy: true,
//    remoteSort: false,
//    autoLoad: false,
//    model: 'gigade.Zip',
//    proxy: {
//        type: 'ajax',
//        url: "/UserPhone/GetYear",
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    }
//});
////生日月
//Ext.define('gigade.Month', {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "month", type: "int"}]
//});
//var MonthStore = Ext.create('Ext.data.Store', {
//    autoDestroy: true,
//    remoteSort: false,
//    autoLoad: false,
//    model: 'gigade.Zip',
//    proxy: {
//        type: 'ajax',
//        url: "/UserPhone/GetYear",
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    }
//});
////生日日
//Ext.define('gigade.Day', {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "day", type: "int"}]
//});

//var DayStore = Ext.create('Ext.data.Store', {
//    autoDestroy: true,
//    remoteSort: false,
//    autoLoad: false,
//    model: 'gigade.Zip',
//    proxy: {
//        type: 'ajax',
//        url: "/UserPhone/GetYear",
//        actionMethods: 'post',
//        reader: {
//            type: 'json',
//            root: 'items'
//        }
//    }
//});

//頁面載入完成
Ext.onReady(function () {
    var addTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 900,
        url: '/Member/SavePhone',
        defaults: {
            labelWidth: 100,
            width: 250,
            margin: '5 0 0 10'
        },
        border: false,
        plain: true,
        id: 'AddUserPForm',
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '會員姓名',
                id: 'name',
                name: 'name',
                allowBlank: false,
                submitValue: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '行動電話',
                id: 'tel',
                name: 'tel',
                allowBlank: false,
                submitValue: true,
                regex: /^09[0-9]{8}$/,
                regexText: '格式不正確,必須是09開頭的10位數字！',
                maxLength: 10,
                maxLengthText: '最大長度為10'
            },
            {
                xtype: "datefield",
                id: 'birth',
                name: 'birth',
                fieldLabel: '生日',
                format: 'Y-m-d',
                allowBlank: false,
                submitValue: true,
                editable: false,              
                value: '1970-01-01'
            },
        //         {
        //             xtype: 'fieldcontainer',
        //             layout: 'hbox',
        //             width: 600,
        //             defaults: {
        //                 hideLabel: true
        //             },
        //             items: [
        //                {
        //                    xtype: 'displayfield',
        //                    value: '生日:',
        //                    labelWidth: 100,
        //                    margin: '3 40 0 0'
        //                },
        //                 {
        //                     xtype: 'combo',
        //                     id: 'year',
        //                     queryMode: 'local',
        //                     displayField: 'year',
        //                     valueField: 'year',
        //                     store: CityStore,
        //                     editable: false,
        //                     queryMode: 'remote',
        //                     allowBlank: true,
        //                     width: 80,
        //                     listeners: {
        //                         "select": function (combo, record) {
        //                             var z = Ext.getCmp("cob_czip");
        //                             z.clearValue();
        //                             ZipStore.removeAll();
        //                             comCity = true;
        //                         }
        //                     }
        //                 },
        //                {
        //                    xtype: 'combo',
        //                    name: 'czip',
        //                    id: 'cob_czip',
        //                    queryMode: 'local',
        //                    displayField: 'small',
        //                    valueField: 'zipcode',
        //                    store: ZipStore,
        //                    editable: false,
        //                    queryMode: 'remote',
        //                    allowBlank: true,
        //                    width: 100,
        //                    margins: '0 5 0 5',
        //                    listeners: {
        //                        beforequery: function (qe) {
        //                            if (comCity) {
        //                                delete qe.combo.lastQuery;
        //                                ZipStore.load({
        //                                    params: {
        //                                        topValue: Ext.getCmp("cob_ccity").getValue()
        //                                    }
        //                                });
        //                                comCity = false;
        //                            }
        //                        },
        //                        "select": function (combo, record) {
        //                            var ArrZip = Ext.getCmp("cob_czip").getRawValue().split('/');
        //                            Ext.getCmp('txt_caddress').setValue(Ext.getCmp("cob_ccity").getRawValue() + ArrZip[1]);
        //                        }
        //                    }
        //                }
        //             ]
        //         },
            {

                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 600,
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'displayfield',
                        value: '地址:',
                        labelWidth: 100,
                        margin: '3 40 0  0'
                    },
                    {
                        xtype: 'combo',
                        id: 'cob_ccity',
                        displayField: 'middle',
                        valueField: 'middlecode',
                        store: CityStore,
                        editable: false,
                        //queryMode: 'local',
                        queryMode: 'remote',
                        allowBlank: true,
                        width: 80,
                        listeners: {
                            "select": function (combo, record) {
                                var z = Ext.getCmp("cob_czip");
                                z.clearValue();
                                ZipStore.removeAll();
                                comCity = true;
                            }
                        }
                    },
                    {
                        xtype: 'combo',
                        name: 'czip',
                        id: 'cob_czip',
                        displayField: 'small',
                        valueField: 'zipcode',
                        store: ZipStore,
                        editable: false,
                        //queryMode: 'local',
                        queryMode: 'remote',
                        allowBlank: true,
                        width: 100,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comCity) {
                                    delete qe.combo.lastQuery;
                                    ZipStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity").getValue()
                                        }
                                    });
                                    comCity = false;
                                }
                            }
                        }
                    },
                    {
                        id: 'txt_caddress',
                        xtype: 'textfield',
                        width: 300,
                        name: 'caddress',
                        allowBlank: true
                    }
                ]
            },
            {
                xtype: 'checkbox',
                fieldLabel: '是否接收簡訊廣告',
                id: 'IsAcceptAd',
                name: 'IsAcceptAd',
                allowBlank: true,
                submitValue: true
            },
            {
                xtype: 'textareafield',
                fieldLabel: '管理員備註',
                id: 'Remark',
                name: 'Remark',
                width: 500,
                height: 100,
                autoScroll: true,
                allowBlank: true,
                submitValue: true
            }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '新增',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            name: Ext.htmlEncode(Ext.getCmp('name').getValue()),
                            tel: Ext.htmlEncode(Ext.getCmp('tel').getValue()),
                            birth: Ext.htmlEncode(Ext.getCmp('birth').getRawValue()),
                            zip: Ext.htmlEncode(Ext.getCmp('cob_czip').getValue()),
                            address: Ext.htmlEncode(Ext.getCmp('txt_caddress').getValue()),
                            IsAcceptAd: Ext.htmlEncode(Ext.getCmp('IsAcceptAd').getValue()),
                            Remark: Ext.htmlEncode(Ext.getCmp('Remark').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg.toString() === "1") {
                                    Ext.Msg.alert(INFORMATION, "電話會員新增成功");
                                }
                                form.reset();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg.toString() === "3") {
                                Ext.Msg.alert(INFORMATION, "電話號碼已重複新增失敗");
                            } else if (result.msg.toString() === "2") {
                                Ext.Msg.alert(INFORMATION, "電話會員新增失敗");
                            } else if (result.msg.toString() === "0") {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        }
                    });
                }
            }
        }]
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: addTab,
        autoScroll: true
    });
});
