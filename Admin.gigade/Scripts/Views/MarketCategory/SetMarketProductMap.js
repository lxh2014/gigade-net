
setMarketProductMap = function (market_category_id, market_category_name) {
    var categoryID = "";
    var categoryName = "";
    var treeStore = Ext.create('Ext.data.TreeStore', {
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/MarketCategory/GetProductCatagory',
            actionMethods: 'post'
        },
        rootVisible: false,
        root: {
            text: GIGADE_TYPE,
            //expanded: true,
            children: []
        }
    });
     //store加載完畢后全部展開解決數據過長滾動條底部不能完全顯示的問題
    treeStore.load({
        callback: function () {
            Ext.getCmp("treePanel").expandAll();
        }
    });
    var treePanel = new Ext.tree.TreePanel({
        id: 'treePanel',
        region: 'west',
        width: 250, //左側樹狀結構的寬度
        border: 0,
        height: 250, //窗口的高度
        padding: '5 0 10 5',
        autoScroll: true,
        store: treeStore,
        listeners: {
            'itemclick': function (view, record, item, index, e) {
                var nodeId = record.raw.id; //获取点击的节点id
                var nodeText = record.raw.text; //获取点击的节点text
                categoryID = nodeId;
                categoryName = nodeText;
                Ext.getCmp('category_id').setValue("[" + categoryID + "]" + categoryName);
            }
        }

    });

    var setFrm = Ext.create('Ext.form.Panel', {
        id: 'setFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        url: '/MarketCategory/SavetMarketProductMap',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [

                 {
                     xtype: 'displayfield',
                     fieldLabel: CATEGORY_NAME,
                     id: 'market_category_id',
                     name: 'market_category_id',
                     value: "[" + market_category_id + "]" + market_category_name
                 },
                  {
                      xtype: 'displayfield',
                      fieldLabel: GIGADE_TYPE,
                      id: 'category_id',
                      name: 'category_id',
                      value: ""
                  },
              treePanel
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (categoryID == "") {
                    Ext.Msg.alert(INFORMATION, SETPRODUCTTIP);
                }
                else {
                    if (form.isValid()) {
                        var count = 0;
                        form.submit({
                            params: {
                                comboFrontCage_hide: Ext.htmlEncode(categoryID),
                                comboMarket_hide: Ext.htmlEncode(market_category_id)

                            },
                            success: function (form, action) {
                                if (action.result.success == true) {
                                    if (action.result.msg > 0) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        setWin.close();
                                    }
                                    if (action.result.msg == -1) {
                                        Ext.Msg.alert(INFORMATION, ALEADY_EXISTS);
                                    }
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });


                    }
                }
            }

        }]
    });

    var setWin = Ext.create('Ext.window.Window', {
        title: MARKET_CATEGORY_RELATIONSHIP,
        id: 'setWin',
        iconCls: "icon-user-edit",
        width: 320,
        height: 400,
        layout: 'fit',
        items: [setFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('setWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {

                setFrm.getForm().reset(); //如果是編輯的話

            }
        }
    });
    setWin.show();
}