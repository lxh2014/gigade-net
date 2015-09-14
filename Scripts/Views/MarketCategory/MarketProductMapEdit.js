editFunction = function (row, store) {
    var leftW = 250; //左側樹狀結構的寬度
    var theight = 220; //窗口的高度
    var categoryId = "";    //吉甲地類別編號
    var categoryName = "";  //吉甲地類別名稱
    //獲取左邊的category樹結構(商品分類store)
    var treeStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/MarketCategory/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: SHIPMENT_TYPE,
            expanded: true, 
            children: []
        }
    });
    treeStore.load();

    var MarketStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/MarketCategory/GetFrontCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: SHIPMENT_TYPE,
            expanded: true,
            children: []
        }
    });
    MarketStore.load();

    var editUserFrm = Ext.create('Ext.form.Panel', {
        id: 'editUserFrm',
        frame: true,
        plain: true,
        //constrain: true,
        //defaultType: 'textfield',
        //  autoScroll: true,
        layout: 'anchor',
        labelWidth: 40,
        url: '/MarketCategory/SavetMarketProductMap',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            //吉甲地類別名稱
                    {
                        xtype: 'combotree',
                        id: 'comboFrontCage',
                        name: 'classname',
                        hiddenname: 'classname',
                        editable: false,
                        submitValue: false,
                        colName: 'category_id',
                        store: treeStore,
                        fieldLabel: GIGADE_TYPE,
                        labelWidth: 90,
                        columnWidth: .9,
                        allowBlank: false
                    },
                    {
                        hidden: true,
                        xtype: 'textfield',
                        id: 'comboFrontCage_hide',
                        name: 'comboFrontCage_hide',
                        listeners: {
                            change: function (txt, newValue, oldValue) {
                                if (newValue) {
                                 
                                }
                            }
                        }
                    },
                    //美安類別名稱
                           {
                               xtype: 'combotree',
                               id: 'comboMarket',
                               name: 'classname',
                               hiddenname: 'classname',
                               editable: false,
                               submitValue: false,
                               colName: 'market_category_id',
                               store: MarketStore,
                               fieldLabel: CATEGORY_NAME,
                               labelWidth: 90,
                               columnWidth: .9,
                               allowBlank: false
                           },
                           {
                                hidden: true,
                               xtype: 'textfield',
                               id: 'comboMarket_hide',
                               name: 'comboMarket_hide',
                               listeners: {
                                   change: function (txt, newValue, oldValue) {
                                       if (newValue) {
                                          // alert(newValue);
                                         
                                       }
                                   }
                               }
                           },

                 {
                    xtype: 'displayfield',
                    fieldLabel: MAP_ID,
                    id: 'map_id',
                    name: 'map_id',
                    padding: '5 0 5 5',
                    hidden: true,
                    width: 100
                }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: SAVE,
            handler: function () {
              
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({ 
                   // 
                        params: {
                            comboFrontCage_hide: Ext.getCmp('comboFrontCage_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboFrontCage_hide').getValue()),
                            comboMarket_hide: Ext.getCmp('comboMarket_hide').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('comboMarket_hide').getValue()),
                            map_id: Ext.getCmp('map_id').getValue()
                        },
                        success: function (form, action) {
                                if (action.result.success == true) {
                                    if (action.result.msg > 0) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        editUserWin.close();
                                        MarketProductMapStore.load();
                                    }
                                    if (action.result.msg == -1) {
                                        Ext.Msg.alert(INFORMATION, ALEADY_EXISTS);
                                    }
                                }
                        },
                        failure: function (form, action) {
                            Ext.Msg.alert(INFORMATION, PROMODISCOUNTTIP);
                        }
                    });
                }
            }
        }]
    });


    var editUserWin = Ext.create('Ext.window.Window', {
        id: 'editUserWin',
        width: 400,
        title: CATEGORY_RELATIONSHIP,
        iconCls: 'icon-user-edit',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [
                 editUserFrm
     ],
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editUserWin').destroy();
                         MarketProductMapStore.destroy();
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
                    editUserFrm.getForm().loadRecord(row); //如果是編輯的話
                  
                }
                else {
                    editUserFrm.getForm().reset(); //如果是新增的話
                }
            }
        }
    });
    if (row != null) {
        if (row.data.market_category_id != "") {//活動限制，訂單會員
            Ext.getCmp('comboMarket_hide').setValue(row.data.market_category_id);
            Ext.getCmp('comboMarket').setValue(row.data.market_category_name);
        } 
        if (row.data.product_category_id != "") {//活動限制，訂單會員
            Ext.getCmp('comboFrontCage_hide').setValue(row.data.product_category_id);
            Ext.getCmp('comboFrontCage').setValue(row.data.category_name);
        }
    }
    editUserWin.show();
}
