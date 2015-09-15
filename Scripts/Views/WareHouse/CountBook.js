Ext.require([
    'Ext.form.*'
]);
var sort = "1";
Ext.onReady(function () {
    var Iloc_idTypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "所有料位", "value": "0" },
            { "txt": "主料位", "value": "S" },
            { "txt": "副料位", "value": "R" }
        ]
    }); 
    var DDLStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "單數", "value": "0" },
            { "txt": "雙數", "value": "1" }
        ]
    });
    Ext.define("gigade.Vendor", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "vendor_id", type: "string" },
            { name: "vendor_name_simple", type: "string" }
        ]
    });
    var VendorConditionStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Vendor',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/SendProduct/GetVendorName",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    var formPanel = Ext.create('Ext.form.Panel', {
        frame: false,
        //width: 430,
        bodyPadding: 30,
        border: false,
        items: [
            {
                html: '<div class="capion">提示：匯出料位盤點報表</div>',
                frame: false,
                border: false
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "走道範圍",
                id: 'Iloc_all',
                labelWidth: 110,
                width: 400,
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: "textfield",
                        id: 'startIloc',
                        name: 'startIloc',
                        allowBlank: true,
                        flex: 5
                    },
                    {
                        xtype: 'displayfield',
                        value: "--",
                        flex: 1
                    },
                    {
                        xtype: "textfield",
                        id: 'endIloc',
                        name: 'endIloc',
                        flex: 5,
                        allowBlank: true
                    }
                ]
            },
            {
                fieldLabel: "所在層數",
                xtype: "textfield",
                id: 'level',
                labelWidth: 110,
                name: 'level',
                allowBlank: true
            },
            {//會員群組和會員條件二擇一
                xtype: 'fieldcontainer',
                fieldLabel: '排序方式',
                combineErrors: true,
                labelWidth: 110,
                margins: '0 200 0 0',
                layout: 'hbox',
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'usr',
                        id: "usr1",
                        listeners: {
                            change: function () {
                                sort = "1";
                                if (sort == "1") {
                                    Ext.getCmp('Firstsd').show();
                                    Ext.getCmp('Firstsd').setDisabled(true);
                                }
                            }
                        }
                    },
                    {
                        html: '<div>直線排序</div>',
                        frame: false,
                        border: false
                    },
                    {
                        xtype: 'displayfield',
                        width: 25
                    },
                    {
                        xtype: 'radiofield',
                        name: 'usr',
                        inputValue: "groupname",
                        //fieldLabel: 'Z排序',
                        id: "usr2",
                        checked: true,
                        listeners: {
                            change: function () {
                                sort = "0";
                                if (sort == "0")
                                {
                                    Ext.getCmp('Firstsd').show();
                                    Ext.getCmp('Firstsd').setDisabled(false);
                                }
                            }
                        }
                    },
                    {
                        html: '<div>Z排序</div>',
                        frame: false,
                        border: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "",
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        width: 115
                    },                   
                    {//直線排序
                        xtype: 'combobox',
                        id: 'Firstsd',
                        name: 'Firstsd',
                        fieldLabel: '',
                        store: DDLStore,
                        hiddenName: 'Firstsd_name',
                        colName: 'Firstsd_value',
                        displayField: 'txt',
                        valueField: 'value',
                        typeAhead: true,
                        width: 113,
                        labelWidth: 110,
                        forceSelection: false,
                        disabled: true,
                        value: "0"
                    }
                ]
            },
            {
                fieldLabel: "供應商編號/名稱",
                xtype: "textfield",
                id: 'vender',
                labelWidth: 110,
                name: 'vender',
                allowBlank: true
            },
            //{
            //    xtype: 'combobox',
            //    id: 'search_vendor',
            //    fieldLabel: '供應商條件',
            //    labelWidth: 110,
            //    queryMode: 'local',
            //    editable: false,
            //    store: VendorConditionStore,
            //    displayField: 'vendor_name_simple',
            //    valueField: 'vendor_id',
            //    listeners: {
            //        beforerender: function () {
            //            VendorConditionStore.load({
            //                callback: function () {
            //                    VendorConditionStore.insert(0, { vendor_id: '0', vendor_name_simple: '所有供應商資料' });
            //                    Ext.getCmp("search_vendor").setValue(VendorConditionStore.data.items[0].data.vendor_id);
            //                }
            //            });
            //        }
            //    }
            //},
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
        window.open("/WareHouse/GetCountBook?level=" + Ext.getCmp('level').getValue() + "&startIloc=" + Ext.getCmp('startIloc').getValue() + "&endIloc=" + Ext.getCmp('endIloc').getValue() + "&sort=" + sort + "&firstsd=" + Ext.getCmp('Firstsd').getValue() + "&vender=" + Ext.getCmp('vender').getValue());
        //+ "&search_vendor=" + Ext.getCmp('search_vendor').getValue());
    }

});
