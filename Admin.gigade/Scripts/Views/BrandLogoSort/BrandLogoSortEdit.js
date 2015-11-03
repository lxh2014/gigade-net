editFunction = function (row, store) {
    var comType = false;
    Ext.define('gigade.CategoryStore', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'category_id', type: 'int' },
            { name: 'category_name', type: 'string' }
        ]
    });
    var CategoryStore = Ext.create("Ext.data.Store", {
        autoLoad: true,
        model: 'gigade.CategoryStore',
        proxy: {
            type: 'ajax',
            url: '/BrandLogoSort/GetCategoryStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.BrandStore', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'brand_id', type: 'int' },
            { name: 'brand_name', type: 'string' }
        ]
    });
    var BrandStore = Ext.create("Ext.data.Store", {
        model: 'gigade.BrandStore',
        proxy: {
            type: 'ajax',
            url: '/BrandLogoSort/GetBrandStore',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    BrandStore.on('beforeload', function () {
        Ext.apply(BrandStore.proxy.extraParams,
            {
                category_id: Ext.getCmp('category_id').getValue(),
            });
    });

    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/BrandLogoSort/SaveBLS',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '編號',
                id: 'blo_id',
                name: 'blo_id',
                hidden: true,
            }, {
                xtype: 'combobox',
                fieldLabel: '品牌館分類',
                store:CategoryStore,
                id: 'category_id',
                name: 'category_id',
                queryMode: 'local',
                displayField: 'category_name',
                valueField: 'category_id',
                typeAhead: true,
                //allowBlank: false,
                editable:false,
                listeners: {
                    select: function (combo, record) {
                        var m = Ext.getCmp("brand_id");
                        m.clearValue();
                        BrandStore.removeAll();
                        BrandStore.load({
                            params: {
                                category_id: Ext.getCmp('category_id').getValue(),
                            }
                        });
                        comType = true;
                    }
                },
            },
            {
                xtype: 'combobox',
                fieldLabel: '品牌名稱',
                store:BrandStore,
                id: 'brand_id',
                name: 'brand_id',
                displayField: 'brand_name',
                valueField:'brand_id',
                queryMode: 'local',
                typeAhead: true,
                //allowBlank: false,
                listeners: {
                    beforequery: function (qe) {
                        if (comType) {
                            delete qe.combo.lastQuery;
                            BrandStore.load({
                                params: {
                                    category_id: Ext.getCmp('category_id').getValue(),
                                }
                            });
                            comType = false;
                        }
                    },
                    select: function () {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        myMask.show();
                        Ext.Ajax.request({
                            url: '/BrandLogoSort/MaxSort',
                            params: {
                                category_id: Ext.getCmp('category_id').getValue(),
                            },
                            success: function (form, action) {
                                myMask.hide();
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    Ext.getCmp('blo_sort').setValue(result.data);
                                }
                            }
                        });
                    }
                }
            },
            {
                xtype: 'displayfield',
                id: 'old_brand_id',
                name: 'old_brand_id',
                hidden:'true',
            },
              {
                xtype: 'numberfield',
                fieldLabel: '排序',
                id: 'blo_sort',
                name: 'blo_sort',
                minValue: 1,
                  value:1,
                maxValue:99999999,
                allowDecimals:false,
                allowBlank: false
            },  
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (Ext.getCmp('category_id').getValue() == null) {
                        Ext.Msg.alert("提示信息", "請選擇品牌館分類");
                        return;
                    }
                    else if (Ext.getCmp('brand_id').getValue() == null) {
                        Ext.Msg.alert("提示信息", "請選擇品牌名稱");
                        return;
                    }
                    if (form.isValid()) {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                        myMask.show();
                        form.submit({
                            params: {
                                blo_id: Ext.htmlEncode(Ext.getCmp('blo_id').getValue()),
                                category_id: Ext.htmlEncode(Ext.getCmp('category_id').getValue()),
                                brand_id: Ext.htmlEncode(Ext.getCmp('brand_id').getValue()),
                                blo_sort: Ext.htmlEncode(Ext.getCmp('blo_sort').getValue()),
                                old_brand_id: Ext.htmlEncode(Ext.getCmp('old_brand_id').getValue()),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                myMask.hide();
                                if (result.success) {
                                    if (result.maxCount) {
                                        Ext.Msg.alert("提示信息", "該品牌分類下已有10筆數據");
                                      
                                    }
                                    if (result.repeatSort) {
                                        Ext.Msg.alert("提示信息", "排序重複");
                                       
                                    }
                                    if (result.repeatData) {
                                        Ext.Msg.alert("提示信息", "數據重複");
                                      
                                    }
                                    if (result.re) {
                                        Ext.Msg.alert("提示信息", "保存成功! ");
                                        store.load();
                                        editWin.close();
                                    }
                                  
                                }
                                else {
                                    Ext.Msg.alert("提示信息", "保存失敗! ");
                                    editWin.close();
                                }
                            },
                            failure: function () {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "出現異常! ");
                                editWin.close();
                            }
                        });
                    }
                }
            },
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "品牌圖片管理",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 360,
        height: 180,
        layout: 'fit',//布局样式
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,//false 禁止調整windows窗體的大小
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: "關閉窗口",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示信息", "是否關閉窗口", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    Ext.getCmp('brand_id').allowBlank =true;
                    Ext.getCmp('category_id').allowBlank = true;
                    Ext.getCmp('category_id').setValue(row.data.category_id);
                    BrandStore.load();
                    Ext.getCmp('category_id').setDisabled(true);
                    Ext.getCmp('old_brand_id').setValue(row.data.brand_id);
                
                }
                else {
                    editFrm.getForm().reset();

                }
            }
        }
    });
    editWin.show();
}