/*
新增：【新促銷】滿額滿件送禮    add by shuangshuang0420j and dongya0410j  201507030929
修改1：
修改2：
*/
var currentRecord = { data: {} };
var pageSize = 19;
var win;
var eventID = "";
Ext.Loader.setConfig({ enabled: true });
Ext.require([
    'Ext.data.*',
    'Ext.util.*',
    'Ext.view.View',
    'Ext.ux.DataView.DragSelector',
    'Ext.ux.DataView.LabelEditor'
]);

//送禮促銷活動列表
Ext.define('GIGADE.PROMOGIFT', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'row_id', type: 'int' },
        { name: 'event_id', type: 'string' },
        { name: 'event_name', type: 'string' },
        { name: 'event_desc', type: 'string' },
        { name: 'site_id', type: 'string' },
        { name: 'site_name', type: 'string' },
        { name: 'user_condition_id', type: 'int' },
        { name: 'condition_type', type: 'int' },
        { name: 'device', type: 'int' },
        { name: 'event_start', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'event_end', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'event_status', type: 'int' },
           { name: 'modify_user', type: 'int' },
            { name: 'user_name', type: 'string' }
    ]
});

var promoGiftListStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMOGIFT',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetPromoAmountGift',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});

promoGiftListStore.on("beforeload", function () {
    Ext.apply(promoGiftListStore.proxy.extraParams, {
        event_id: Ext.getCmp("grid_event_id") ? Ext.getCmp("grid_event_id").getValue() : '',
        event_name: Ext.getCmp('grid_event_name') ? Ext.getCmp('grid_event_name').getValue() : ''
    })
})


Ext.define('GIGADE.PROMODETAIL', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'event_id', type: 'string' },
        { name: 'gift_id', type: 'int' },
        { name: 'gift_name', type: 'string' },
        { name: 'product_id', type: 'int' },
        { name: 'product_name', type: 'string' },
        { name: 'gift_type', type: 'int' },
         { name: 'product_num', type: 'int' },
        { name: 'gift_ware', type: 'int' },
        { name: 'gift_num', type: 'int' },
          { name: 'quantity', type: 'int' },
        { name: 'amount', type: 'int' }
    ]
});

var promoDetailStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.PROMODETAIL',
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetPromoGiftDetail',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});


Ext.onReady(function () {
    treeCateStore.load();
    //  SiteStore.load();
    UserConStore.load();
    ConTypeStore.load();

    Ext.QuickTips.init();
    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'west',//左西
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            width: 400,
            margins: '5 4 5 5',
            id: 'west-region-container',
            layout: 'anchor',
            items: promoAmountGiftList
        }
        ,
        {
            region: 'center',//中間
            id: 'center-region-container',
            xtype: 'panel',
            frame: false,
            layout: 'fit',
            width: 500,
            margins: '5 4 5 5',
            items: center
        }
        ],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        renderTo: Ext.getBody()
    });

});

//左邊課程列表
var promoAmountGiftList = Ext.create('Ext.grid.Panel', {
    id: ' promoAmountGiftList',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    store: promoGiftListStore,
    dockedItems: [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [{
            xtype: 'textfield',
            fieldLabel: '活動編號',
            id: 'grid_event_id',
            name: 'grid_event_id',
            margin: '3 0 0 0',
            width: 210,
            labelWidth: 60,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Search();
                    }
                }
            }
        }, {
            xtype: 'textfield',
            fieldLabel: '活動名稱',
            id: 'grid_event_name',
            name: 'grid_event_name',
            width: 210,
            labelWidth: 60,
            margin: '3 0 5 0',
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                        Search();
                    }
                }
            }
        }, {
            xtype: 'button',
            text: '查詢',
            id: 'grid_btn_search',
            iconCls: 'ui-icon ui-icon-search',
            margin: ' 0 0 5 10',
            width: 65,
            handler: Search
        }, {
            xtype: 'button',
            text: '新增活動',
            id: 'grid_btn_add',
            iconCls: 'ui-icon ui-icon-add',
            margin: ' 0 0 5 10',
            width: 75,
            handler: function () {
                var record = { data: { 'row_id': 0 } };
                currentRecord = record;
                Ext.getCmp('west-region-container').setDisabled(true);
                Ext.getCmp('center').getForm().reset();
                promoDetailStore.removeAll();
                ac_store.removeAll();
            }
        }]
    }],
    columns: [
        { header: '活動編號', dataIndex: 'event_id', align: 'left', width: 60, menuDisabled: true, sortable: false },
        { header: '活動名稱', dataIndex: 'event_name', align: 'left', menuDisabled: true, sortable: false, flex: 1 },
        { header: '開始時間', dataIndex: 'event_start', align: 'center', width: 70, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') },
        { header: '結束時間', dataIndex: 'event_end', align: 'center', width: 70, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') },
         { header: '異動者', dataIndex: 'user_name', align: 'left', width: 40, menuDisabled: true, sortable: false },
        {
            header: '狀態', dataIndex: 'event_status', align: 'center', width: 40, menuDisabled: true, sortable: false,
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "<img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                } else if (value == 0) {
                    return "<img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                }
            }
        }
    ],

    bbar: Ext.create('Ext.PagingToolbar', {
        store: promoGiftListStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (view, record, item, index, e) {
            if (e.getTarget('.x-grid-cell').cellIndex != 5) {
                LoadDetail(currentRecord = record);
            }
            else {
                Ext.MessageBox.confirm("提示", Ext.String.format("是否要{0}該活動?", record.data.event_status == 1 ? "禁用" : "啟用"), function (btn) {
                    if (btn == "yes") {
                        UpdateActive(record);
                    }
                    else {
                        return false;
                    }
                });
            }

        },
        resize: function () {
            this.doLayout();
        }
    }
})

//複選框列
var cbm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("edit").setDisabled(selections.length == 0);
            Ext.getCmp("delete").setDisabled(selections.length == 0);
        }
    }
});

var center = Ext.create('Ext.form.Panel', {
    id: 'center',
    autoScroll: true,
    border: false,
    frame: false,
    layout: { type: 'vbox', align: 'stretch' },
    defaults: { margin: '2 2 2 2' },
    items: [
        {
            flex: 2.0,
            title: '基本資料',
            autoScroll: true,
            frame: false,
            items: [
                {
                    xtype: 'container',
                    autoScroll: true,
                    defaults: { margin: '0 5 5 10', labelWidth: 60, autoScroll: true, width: 1150 },
                    items: [

                        {
                            xtype: 'container',
                            layout: 'hbox',
                            autoScroll: true,
                            defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                            id: 'part1',
                            items: [{
                                xtype: 'textfield',
                                id: 'event_name',
                                fieldLabel: '活動名稱',
                                margin: '3 5 5 0',
                                allowBlank: false
                            }, {
                                xtype: 'textfield',
                                id: 'event_desc',
                                allowBlank: false,
                                fieldLabel: '活動描述'
                            }, {
                                xtype: 'datetimefield',
                                format: 'Y-m-d H:i:s',
                                time: { hour: 00, min: 00, sec: 00 },//開始時間00：00：00
                                id: 'event_start',
                                labelWidth: 60,
                                width: 220,
                                allowBlank: false,
                                editable: false,
                                celleditable: false,
                                minValue: new Date(),
                                fieldLabel: '開始時間',
                                listeners: {
                                    select: function (a, b, c) {
                                        var start = Ext.getCmp("event_start");
                                        var end = Ext.getCmp("event_end");
                                        if (end.getValue() == null) {
                                            end.setValue(setNextMonth(start.getValue(), 1));
                                        } else if (end.getValue() < start.getValue()) {
                                            Ext.Msg.alert(INFORMATION, DATA_TIP);
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }
                                    }
                                }
                            }, {
                                xtype: 'datetimefield',
                                format: 'Y-m-d H:i:s',
                                time: { hour: 23, min: 59, sec: 59 },
                                id: 'event_end',
                                labelWidth: 60,
                                width: 220,
                                allowBlank: false,
                                editable: false,
                                celleditable: false,
                                minValue: new Date(),
                                fieldLabel: '結束時間',
                                listeners: {
                                    select: function (a, b, c) {
                                        var start = Ext.getCmp("event_start");
                                        var end = Ext.getCmp("event_end");
                                        if (start.getValue() != "" && start.getValue() != null) {
                                            if (end.getValue() < start.getValue()) {
                                                Ext.Msg.alert(INFORMATION, DATA_TIP);
                                                end.setValue(setNextMonth(start.getValue(), 1));
                                            }
                                        }
                                        else {
                                            start.setValue(setNextMonth(end.getValue(), -1));
                                        }
                                    }
                                }
                            }]
                        },
                         {
                             xtype: 'container',
                             layout: 'hbox',
                             id: 'part2',
                             defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                             items: [
                                   {
                                       xtype: 'combobox', //Wibset
                                       allowBlank: false,
                                       editable: false,
                                       fieldLabel: "活動站台",
                                       margin: '3 5 5 0',
                                       id: 'site_id',
                                       name: 'site_id',
                                       hiddenName: 'site_id',
                                       store: SiteStore,
                                       displayField: 'Site_Name',
                                       valueField: 'Site_Id',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               SiteStore.load();
                                           }
                                       }
                                   }
                              ,
                               {

                                   xtype: 'combobox',
                                   allowBlank: false,
                                   fieldLabel: '會員條件',
                                   id: 'user_condition_id',
                                   name: 'user_condition_id',
                                   store: UserConStore,
                                   displayField: 'condi_name',
                                   valueField: 'condi_id',
                                   hiddenname: 'condi_id',
                                   typeAhead: true,
                                   queryMode: 'local',
                                   lastQuery: '',
                                   emptyText: "請選擇",
                                   forceSelection: false,
                                   listeners: {
                                       blur: function (combo, eOpts) {
                                           var val = combo.getRawValue();
                                           if (val != "") {
                                               var store = combo.store;
                                               if (store.find("condi_name", val) == -1) {
                                                   combo.reset();
                                                   combo.isValid();
                                                   Ext.Msg.alert(INFORMATION, "該會員條件不存在！");

                                               }
                                           }
                                       }

                                   }
                               },
                              {
                                  xtype: 'radiogroup',
                                  hidden: false,
                                  id: 'device',
                                  name: 'device',
                                  fieldLabel: "裝置",
                                  colName: 'device',
                                  width: 280,
                                  defaults: {
                                      name: 'device',
                                      margin: '0 8 0 0'
                                  },
                                  columns: 3,
                                  vertical: true,
                                  items: [
                                  { boxLabel: "不分", id: 'bufen', inputValue: '0', checked: true },
                                  { boxLabel: "PC", id: 'pc', inputValue: '1' },
                                  { boxLabel: "手機/平板", id: 'mobilepad', inputValue: '4' }
                                  ]
                              }

                             ]
                         },
                        {
                            xtype: 'container',
                            layout: 'hbox',
                            autoScroll: true,
                            defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                            id: 'part3',
                            items: [
                               {
                                   xtype: 'combobox',
                                   allowBlank: false,
                                   editable: false,
                                   fieldLabel: '商品設定',
                                   margin: '3 5 5 0',
                                   id: 'condition_type',
                                   name: 'condition_type',
                                   store: ConTypeStore,
                                   displayField: 'condi_name',
                                   valueField: 'condi_id',
                                   typeAhead: true,
                                   lastQuery: '',
                                   emptyText: "請選擇",
                                   forceSelection: false,
                                   listeners: {
                                       change: function (combo, records, eOpts) {
                                           var value = Ext.getCmp("condition_type").getValue();
                                           Ext.getCmp('p_brand').setValue("").hide();
                                           Ext.getCmp('p_class').setValue("").hide();
                                           Ext.getCmp('p_category').hide();
                                           Ext.getCmp('p_product').hide();
                                           Ext.getCmp('p_product_id').setValue("").hide();
                                           Ext.getCmp('p_cart').setValue("").hide();
                                           Ext.getCmp('p_payment').setValue("").hide();
                                           Ext.getCmp('category_name').setValue("").hide();
                                           Ext.getCmp('category_id').setValue("").hide();
                                           ac_store.removeAll();
                                           if (value == "1") {
                                               Ext.getCmp('p_brand').show();
                                           } else if (value == "2") {
                                               Ext.getCmp('category_name').show();
                                               Ext.getCmp('category_id').show();
                                               Ext.getCmp('p_category').show();
                                           } else if (value == "3") {
                                               Ext.getCmp('p_class').show();
                                           } else if (value == "4") {
                                               Ext.getCmp("p_product_id").show();
                                               Ext.getCmp("p_product").show();
                                           } else if (value == "5") {
                                               Ext.getCmp('p_cart').show();
                                           } else if (value == "6") {
                                               Ext.getCmp('p_payment').show();
                                           }
                                       }
                                   }
                               },
                                 {
                                     xtype: 'combobox', //Wibset
                                     editable: false,
                                     hidden: true,
                                     fieldLabel: "活動品牌",
                                     margin: '3 5 5 10',
                                     id: 'p_brand',
                                     name: 'p_brand',
                                     hiddenName: 'p_brand',
                                     store: BrandStore,
                                     displayField: 'brand_name',
                                     valueField: 'brand_id',
                                     typeAhead: true,
                                     forceSelection: false,
                                     queryMode: 'local',
                                     multiSelect: true, //多選
                                     emptyText: "請選擇",
                                     listeners: {
                                         beforequery: function (qe) {
                                             delete qe.combo.lastQuery;
                                             BrandStore.load({
                                                 params: {
                                                     isShowGrade: 1
                                                 }
                                             });
                                         }
                                     }
                                 },
                                  {
                                      xtype: 'combobox', //Wibset
                                      editable: false,
                                      hidden: true,
                                      fieldLabel: "活動館別",
                                      margin: '3 5 5 10',
                                      id: 'p_class',
                                      name: 'p_class',
                                      hiddenName: 'class_id',
                                      store: ShopClassStore,
                                      displayField: 'class_name',
                                      valueField: 'class_id',
                                      typeAhead: true,
                                      forceSelection: false,
                                      queryMode: 'local',
                                      multiSelect: true, //多選
                                      emptyText: "請選擇",
                                      listeners: {
                                          beforequery: function (qe) {
                                              delete qe.combo.lastQuery;
                                              ShopClassStore.load();
                                          }
                                      }
                                  },
                                   {
                                       xtype: 'combobox', //Wibset
                                       editable: false,
                                       hidden: true,
                                       fieldLabel: "付款方式",
                                       margin: '3 5 5 10',
                                       id: 'p_payment',
                                       name: 'p_payment',
                                       hiddenName: 'parameterCode',
                                       store: PaymentStore,
                                       displayField: 'parameterName',
                                       valueField: 'parameterCode',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               PaymentStore.load();
                                           }
                                       }
                                   },
                                   {
                                       xtype: 'combobox', //Wibset
                                       editable: false,
                                       hidden: true,
                                       fieldLabel: "購物車",
                                       margin: '3 5 5 10',
                                       id: 'p_cart',
                                       name: 'p_cart',
                                       hiddenName: 'cart_id',
                                       store: ShopCartStore,
                                       displayField: 'cart_name',
                                       valueField: 'cart_id',
                                       typeAhead: true,
                                       forceSelection: false,
                                       queryMode: 'local',
                                       multiSelect: true, //多選
                                       emptyText: "請選擇",
                                       listeners: {
                                           beforequery: function (qe) {
                                               delete qe.combo.lastQuery;
                                               ShopCartStore.load();
                                           }
                                       }

                                   },
                                       {
                                           xtype: 'textfield',
                                           fieldLabel: "商品編號",
                                           hidden: true,
                                           readOnly: true,
                                           id: 'p_product_id',
                                           name: 'p_product_id',
                                           width: 200
                                       },
                                       {
                                           xtype: 'textfield',
                                           fieldLabel: "商品名稱",
                                           hidden: true,
                                           readOnly: true,
                                           id: 'p_product_name',
                                           name: 'p_product_name',
                                           width: 200
                                       },
                                    {//商品設定
                                        xtype: 'button',
                                        text: "選擇商品",
                                        hidden: true,
                                        id: 'p_product',
                                        name: 'p_product',
                                        width: 80,
                                        handler: function () {
                                            var selectproductid = Ext.getCmp("p_product_id").getValue();
                                            PromationMationShow();
                                        }
                                    },
                            {
                                xtype: 'button',
                                text: "請選擇類別",
                                hidden: true,
                                id: 'p_category',
                                name: 'p_category',
                                width: 100,
                                handler: function () {
                                    var categoryid = Ext.getCmp("category_id").getValue();
                                    var categoryname = Ext.getCmp("category_name").getValue();
                                    SelectImgCate(categoryid, categoryname, treeCateStore);
                                }
                            },
                              {//類別id
                                  xtype: 'displayfield',
                                  fieldLabel: "類別ID",
                                  editable: false,
                                  hidden: true,
                                  id: 'category_id',
                                  name: 'category_id'
                              },
                              {//類別名稱
                                  xtype: 'displayfield',
                                  fieldLabel: "類別名稱",
                                  hidden: true,
                                  editable: false,
                                  id: 'category_name',
                                  name: 'category_name',
                                  width: 500
                              }
                            ]
                        }
                        ,
                       {
                           flex: 3.0,
                           title: '贈品細項',
                           xtype: 'gridpanel',
                           id: 'detailist',
                           autoScroll: true,
                           frame: false,
                           store: promoDetailStore,
                           columns: [
                               { header: '序號', xtype: 'rownumberer', width: 46, align: 'center' },
                               { header: '贈品名稱', dataIndex: 'gift_name', align: 'left', flex: 1, menuDisabled: true, sortable: false },
                                {
                                    header: '贈品類型', dataIndex: 'gift_type', align: 'left', width: 120, menuDisabled: true, sortable: false,
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        if (value == 1) {
                                            return "商品";
                                        }
                                        else if (value == 2) {
                                            return "購物金";
                                        }
                                        else if (value == 3) {
                                            return "抵用券";
                                        }
                                    }
                                },
                                {
                                    header: '商品編號', dataIndex: 'product_id', align: 'left', width: 120, menuDisabled: true, sortable: false,
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        if (value == 0) {
                                            return "";
                                        }
                                        else {
                                            return value;
                                        }

                                    }
                                },
                               {
                                   header: '贈送數量', dataIndex: 'product_num', align: 'left', width: 60, menuDisabled: true, sortable: false,
                                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                       if (record.data.gift_type != 1) {
                                           return "";
                                       }
                                       else {
                                           return value;
                                       }

                                   }
                               },
                                 { header: '贈品額度', dataIndex: 'gift_ware', align: 'left', width: 60, menuDisabled: true, sortable: false },
                               {
                                   header: '贈品倍數', dataIndex: 'gift_num', align: 'left', width: 60, menuDisabled: true, sortable: false,
                                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                       if (record.data.gift_type == 1) {
                                           return 0;
                                       }
                                       else {
                                           return value;
                                       }

                                   }
                               },
                               { header: '滿件數量', dataIndex: 'quantity', align: 'left', width: 60, menuDisabled: true, sortable: false },
                               { header: '滿額金額', dataIndex: 'amount', align: 'left', width: 60, menuDisabled: true, sortable: false }
                           ],
                           tbar: [
                               {
                                   text: '新增', id: 'add', handler: function () {
                                       detailAdd(null, promoDetailStore);
                                   }
                               },
                               { text: '修改', id: 'edit', handler: detailEdit, disabled: true },
                               {
                                   text: '刪除', id: 'delete', disabled: true, handler: function () {
                                       detailDelete(promoDetailStore);
                                   }
                               }],
                           selModel: cbm
                       }
                    ]
                }]
        }],
    bbar: [{
        text: '保存',
        id: 'btn_save',
        iconCls: 'ui-icon ui-icon-checked'
        ,
        handler: Save
    }, {
        text: '重置',
        id: 'btn_reset',
        iconCls: 'ui-icon ui-icon-reset',
        handler: function () { LoadDetail(currentRecord); }
    }, {
        text: '取消',
        id: 'btn_cancel',
        iconCls: 'ui-icon ui-icon-cancel',
        handler: function () {
            Ext.getCmp('west-region-container').setDisabled(false);
        }
    }]
})


function detailAdd(row, store) {

    GiftTypeStore.load();
    var detailAddFrm = Ext.create('Ext.form.Panel', {
        id: 'detailAddFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        defaults: { msgTarget: "side", labelWidth: 80 },
        items: [
             {
                 xtype: 'textfield',
                 fieldLabel: '贈品編號',
                 hidden: true,
                 id: 'gift_id',
                 name: 'gift_id',
                 width: 310
             },
             {
                 xtype: 'textfield',
                 fieldLabel: '贈品名稱',
                 id: 'gift_name',
                 name: 'gift_name',
                 allowBlank: false,
                 width: 310
             },
                 {
                     xtype: 'combobox',
                     allowBlank: false,
                     editable: false,
                     fieldLabel: '贈品類型',
                     width: 310,
                     id: 'gift_type',
                     name: 'gift_type',
                     store: GiftTypeStore,
                     displayField: 'text',
                     valueField: 'value',
                     typeAhead: true,
                     lastQuery: '',
                     emptyText: "請選擇",
                     forceSelection: false,
                     listeners: {
                         select: function (combo, records, eOpts) {
                             var value = Ext.getCmp("gift_type").getValue();
                             Ext.getCmp("g_product_id").hide();
                             Ext.getCmp("g_product_id").allowBlank = true;
                             Ext.getCmp("g_product_name").allowBlank = true;
                             Ext.getCmp("g_product_name").hide();
                             Ext.getCmp("g_num").hide();
                             Ext.getCmp("g_num").allowBlank = true;


                             Ext.getCmp("gift_ware").hide();
                             Ext.getCmp("gift_ware").allowBlank = true;
                             Ext.getCmp("gift_num").hide();
                             Ext.getCmp("gift_num").allowBlank = true;
                             switch (value) {
                                 case "1":
                                     Ext.getCmp("g_product_id").show();
                                     Ext.getCmp("g_product_id").allowBlank = false;
                                     Ext.getCmp("g_product_name").show();
                                     Ext.getCmp("g_product_name").allowBlank = false;
                                     Ext.getCmp("g_num").show();
                                     Ext.getCmp("g_num").allowBlank = false;
                                     break;
                                 default:
                                     Ext.getCmp("gift_ware").show();
                                     Ext.getCmp("gift_ware").allowBlank = false;
                                     Ext.getCmp("gift_num").show().setValue(1);
                                     Ext.getCmp("gift_num").allowBlank = false;
                                     break;
                             }
                         }
                     }
                 },
                   {
                       xtype: 'numberfield',
                       fieldLabel: '商品編號',
                       id: 'g_product_id',
                       name: 'g_product_id',
                       minValue: 10000,
                       hidden: true,
                       allowDecimals: false,
                       allowBlank: true,
                       width: 310,
                       listeners: {
                           'blur': function () {
                               var pid = Ext.getCmp("g_product_id");
                               var pname = Ext.getCmp("g_product_name");
                               if (pid.getValue() != "" && pid.getValue() != null) {
                                   Ext.Ajax.request({
                                       url: '/EventPromo/GetProdName',
                                       params: {
                                           product_id: Ext.htmlEncode(pid.getValue())
                                       },
                                       success: function (form, action) {
                                           var result = Ext.decode(form.responseText);
                                           if (result.success) {
                                               pname.setValue(result.prod_name);
                                           }
                                           else {
                                               Ext.Msg.alert(INFORMATION, "獲取商品名稱失敗！");
                                               pname.setValue("");
                                           }
                                       },
                                       failure: function () {
                                           Ext.Msg.alert(INFORMATION, "獲取商品名稱失敗！");
                                           pname.setValue("");
                                       }
                                   });
                               }
                               else {
                                   pname.setValue("");
                               }

                           }
                       }
                   },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '商品名稱',
                        id: 'g_product_name',
                        name: 'g_product_name',
                        hidden: true,
                        allowBlank: true,
                        width: 310
                    },
                      {
                          xtype: 'numberfield',
                          fieldLabel: '贈送數量(次)',
                          id: 'g_num',
                          name: 'g_num',
                          value: 1,
                          minValue: 0,
                          hidden: true,
                          allowDecimals: false,
                          allowBlank: true,
                          width: 310
                      },
                       {
                           xtype: 'numberfield',
                           fieldLabel: '贈送額度',
                           id: 'gift_ware',
                           name: 'gift_ware',
                           minValue: 0,
                           hidden: true,
                           allowDecimals: false,
                           allowBlank: true,
                           width: 310
                       },
                       {
                           xtype: 'numberfield',
                           fieldLabel: '贈送倍數',
                           id: 'gift_num',
                           name: 'gift_num',
                           value: 1,
                           minValue: 0,
                           hidden: true,
                           allowDecimals: false,
                           allowBlank: true,
                           width: 310
                       },
                        {
                            xtype: 'numberfield',
                            fieldLabel: '滿件數量',
                            id: 'quantity',
                            name: 'quantity',
                            minValue: 0,
                            value: 0,
                            allowBlank: false,
                            allowDecimals: false,
                            width: 310
                        },
                       {
                           xtype: 'numberfield',
                           fieldLabel: '滿額金額',
                           id: 'amount',
                           name: 'amount',
                           value: 0,
                           minValue: 0,
                           allowBlank: false,
                           allowDecimals: false,
                           width: 310
                       }],
        buttons: [{
            text: '重置',
            handler: function () {
                if (row) {
                    Ext.getCmp("g_product_id").hide();
                    Ext.getCmp("g_product_id").allowBlank = true;
                    Ext.getCmp("g_product_name").allowBlank = true;
                    Ext.getCmp("g_product_name").hide();
                    Ext.getCmp("g_num").hide();
                    Ext.getCmp("g_num").allowBlank = true;

                    Ext.getCmp("gift_ware").allowBlank = true;
                    Ext.getCmp("gift_ware").hide();
                    Ext.getCmp("gift_num").allowBlank = true;
                    Ext.getCmp("gift_num").hide();
                    if (row.data.gift_type == 1) {
                        Ext.getCmp("g_product_id").show();
                        Ext.getCmp("g_product_id").allowBlank = false;
                        Ext.getCmp("g_product_name").show();
                        Ext.getCmp("g_product_name").allowBlank = false;
                        Ext.getCmp("g_num").show();
                        Ext.getCmp("g_num").allowBlank = false;
                        Ext.getCmp("gift_num").setValue("1");
                    }
                    else {
                        Ext.getCmp("gift_ware").show();
                        Ext.getCmp("gift_ware").allowBlank = false;
                        Ext.getCmp("gift_num").show();
                        Ext.getCmp("gift_num").allowBlank = false;
                    }
                    detailAddFrm.getForm().loadRecord(row);
                }
                else {
                    this.up('form').getForm().reset();
                }
            }
        }, {
            text: '確定',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid())
                    //驗證商品是否通過審核
                    if (Ext.getCmp("gift_type").getValue() == "1" && (Ext.getCmp("g_product_name").getValue() == "" || Ext.getCmp("g_product_name").getValue().indexOf("不可選擇組合商品") > -1)) {
                        Ext.Msg.alert(INFORMATION, "請輸入有效的商品編號！");
                    }
                    else {
                        if (row) {
                            row.set('gift_name', Ext.getCmp("gift_name").getValue());
                            row.set('gift_type', Ext.getCmp("gift_type").getValue());
                            if (Ext.getCmp("gift_type").getValue() == "1") {
                                row.set('product_id', Ext.getCmp("g_product_id").getValue());
                                row.set('product_num', Ext.getCmp("g_num").getValue());
                                row.set('product_name', Ext.getCmp("g_product_name").getValue());
                                row.set('gift_ware', 0);
                                row.set('gift_num', 0);
                            } else {
                                row.set('product_id', "");
                                row.set('product_num', "");
                                row.set('product_name', "");
                                row.set('gift_ware', Ext.getCmp("gift_ware").getValue());
                                row.set('gift_num', Ext.getCmp("gift_num").getValue());
                            }
                            row.set('quantity', Ext.getCmp("quantity").getValue());
                            row.set('amount', Ext.getCmp("amount").getValue());
                        }
                        else {
                            store.add({
                                event_id: currentRecord.data.event_id,
                                gift_id: Ext.getCmp("gift_id").getValue(),
                                gift_name: Ext.getCmp("gift_name").getValue(),
                                gift_type: Ext.getCmp("gift_type").getValue(),
                                product_id: Ext.getCmp("gift_type").getValue() == "1" ? Ext.getCmp("g_product_id").getValue() : "",
                                product_num: Ext.getCmp("gift_type").getValue() == "1" ? Ext.getCmp("g_num").getValue() : "",
                                product_name: Ext.getCmp("gift_type").getValue() == "1" ? Ext.getCmp("g_product_name").getValue() : "",
                                gift_ware: Ext.getCmp("gift_type").getValue() == "1" ? "" : Ext.getCmp("gift_ware").getValue(),
                                gift_num: Ext.getCmp("gift_type").getValue() == "1" ? "" : Ext.getCmp("gift_num").getValue(),
                                quantity: Ext.getCmp("quantity").getValue(),
                                amount: Ext.getCmp("amount").getValue()
                            });
                        }
                        detailAddWin.destroy();
                    }
            }

        }]
    })

    var detailAddWin = Ext.create('Ext.window.Window', {
        title: '贈品詳情',
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783 + 20,
        layout: 'fit',
        items: [detailAddFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                if (row) {
                    detailAddFrm.getForm().loadRecord(row);
                    if (row.data.gift_type == 1) {
                        Ext.getCmp("g_product_id").setValue(row.data.product_id).show();
                        Ext.getCmp("g_product_id").allowBlank = false;
                        Ext.getCmp("g_product_name").setValue(row.data.product_name).show();
                        Ext.getCmp("g_num").setValue(row.data.product_num).show();
                        Ext.getCmp("g_num").allowBlank = false;
                    }
                    else {
                        Ext.getCmp("gift_ware").show();
                        Ext.getCmp("gift_ware").allowBlank = false;
                        Ext.getCmp("gift_num").show();
                        Ext.getCmp("gift_num").allowBlank = false;
                    }
                } else {
                    detailAddFrm.getForm().reset();
                }
            }
        }
    })
    detailAddWin.show();

}

function detailEdit() {

    var sms = Ext.getCmp("detailist").getSelectionModel().getSelection();
    if (sms.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (sms.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (sms.length == 1) {
        detailAdd(sms[0], promoDetailStore);
    }

}



function detailDelete(store) {
    var row = Ext.getCmp("detailist").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
        if (btn == 'yes') {
            store.remove(row);
        }
    });
}


function LoadDetail(record) {

    if (record.data.row_id == undefined || record.data.row_id == 0) {
        Ext.getCmp('center').getForm().reset();
        promoDetailStore.removeAll();
        ac_store.removeAll();
    } else {
        center.getForm().loadRecord(record);
        switch (record.data.device) {
            case 0:
                Ext.getCmp("bufen").setValue(true);
                break;
            case 1:
                Ext.getCmp("pc").setValue(true);
                break;
            case 4:
                Ext.getCmp("mobilepad").setValue(true);
                break;
        }


        SiteStore.load({
            callback: function () {
                var siteIDs = record.data.site_id.split(',');
                var arrTemp = new Array();
                for (var i = 0; i < siteIDs.length; i++) {
                    arrTemp.push(SiteStore.getAt(SiteStore.find("Site_Id", siteIDs[i])));
                }
                Ext.getCmp('site_id').setValue(arrTemp);
            }
        });
        //根據促銷設定分別抓取設定的值
        var condiStr = "";
        Ext.Ajax.request({
            url: '/EventPromo/GetCondiType',
            method: 'post',
            async: false, //true為異步，false為同步
            params: {
                event_id: record.data.event_id,
                condiType: record.data.condition_type
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    condiStr = result.conStr;
                }
            }
        });

        switch (record.data.condition_type) {
            case 1:
                BrandStore.load({
                    params: {
                        isShowGrade: 1
                    },
                    callback: function () {
                        var brandIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < brandIDs.length; i++) {
                            arrTemp.push(BrandStore.getAt(BrandStore.find("brand_id", brandIDs[i])));
                        }
                        Ext.getCmp('p_brand').setValue(arrTemp);
                    }
                })
                break;
            case 2://設定類別
                var treeIds = condiStr.split(',');

                var category_ids = "";
                var category_names = "";
                for (var i = 0; i < treeIds.length; i++) {
                    var cats = treeIds[i].split('&');
                    category_ids += cats[0] + ",";
                    category_names += cats[1] + ",";
                }
                if (category_ids != "") {
                    category_ids = category_ids.substring(0, category_ids.lastIndexOf(','));
                }
                if (category_names != "") {
                    category_names = category_names.substring(0, category_names.lastIndexOf(','));
                }
                Ext.getCmp('category_name').setValue(category_names);
                Ext.getCmp('category_id').setValue(category_ids);
                break;
            case 3:
                ShopClassStore.load({
                    callback: function () {
                        var classIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < classIDs.length; i++) {
                            arrTemp.push(ShopClassStore.getAt(ShopClassStore.find("class_id", classIDs[i])));
                        }
                        Ext.getCmp('p_class').setValue(arrTemp);
                    }
                })
                break;
            case 4:
                ac_store.removeAll();
                var ac_storeIDs = condiStr.split(',');//獲取到的是已11111&3,....類似這樣的形式
                var resultproductid = "";
                for (var i = 0; i < ac_storeIDs.length; i++) {
                    var product_names_two = "";
                    var results = ac_storeIDs[i].split('&');
                    var t_product_id = results[0];
                    var t_product_num = results[1];
                    resultproductid = resultproductid + t_product_id + ',';//product_id相互連接
                    product_names_two = results[2];
                    ac_store.add({
                        product_id: t_product_id,
                        product_name: product_names_two,
                        product_num: t_product_num
                    });
                }
                if (resultproductid.indexOf(',') >= 0) {
                    Ext.getCmp('p_product_id').setValue(resultproductid.substring(0, resultproductid.lastIndexOf(',')));
                }
                Ext.getCmp('p_product_name').setValue(condiStr);
                break;
            case 5:
                ShopCartStore.load({
                    callback: function () {
                        var cartIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < cartIDs.length; i++) {
                            arrTemp.push(ShopCartStore.getAt(ShopCartStore.find("cart_id", cartIDs[i])));
                        }
                        Ext.getCmp('p_cart').setValue(arrTemp);
                    }
                })
                break;
            case 6:
                PaymentStore.load({
                    callback: function () {
                        var payIDs = condiStr.split(',');
                        var arrTemp = new Array();
                        for (var i = 0; i < payIDs.length; i++) {
                            arrTemp.push(PaymentStore.getAt(PaymentStore.find("parameterCode", payIDs[i])));
                        }
                        Ext.getCmp('p_payment').setValue(arrTemp);
                    }
                })
                break;
        }
        promoDetailStore.load({
            params: {
                event_id: record.data.event_id
            }
        });
    }


}


function Search() {
    Ext.getCmp('grid_event_id').setValue(Ext.getCmp('grid_event_id').getValue().replace(/\s+/g, ","));
    if (!Ext.getCmp('grid_event_id').isValid()) return;
    promoGiftListStore.removeAll();
    promoGiftListStore.loadPage(1);
}


function Save() {

    if (Ext.getCmp('center').getForm().isValid()) {
        if (promoDetailStore.data.length <= 0) {
            Ext.Msg.alert(INFORMATION, '請將細項資料填寫完整');
            return;
        }

        var myMask = new Ext.LoadMask(Ext.getBody(), {
            msg: 'Loading...'
        });
        myMask.show();

        var promoAmountGift = {};
        promoAmountGift.row_id = currentRecord.data.row_id == null ? 0 : currentRecord.data.row_id;
        promoAmountGift.event_name = Ext.htmlEncode(Ext.getCmp('event_name').getValue());
        promoAmountGift.event_desc = Ext.htmlEncode(Ext.getCmp('event_desc').getValue());
        promoAmountGift.site_id = Ext.htmlEncode(Ext.getCmp('site_id').getValue());
        promoAmountGift.user_condition_id = Ext.htmlEncode(Ext.getCmp('user_condition_id').getValue());
        promoAmountGift.condition_type = Ext.htmlEncode(Ext.getCmp('condition_type').getValue());
        promoAmountGift.device = Ext.htmlEncode(Ext.getCmp('device').getValue().device);
        promoAmountGift.event_start = Ext.Date.format(new Date(Ext.getCmp('event_start').getValue()), 'Y-m-d H:i:s');
        promoAmountGift.event_end = Ext.Date.format(new Date(Ext.getCmp('event_end').getValue()), 'Y-m-d H:i:s');
        promoAmountGift.event_status = 0;//改動后默認狀態為停用


        var promoDetail = [];

        for (var i = 0, j = promoDetailStore.data.length ; i < j; i++) {
            var record = promoDetailStore.data.items[i];

            promoDetail.push({
                'gift_id': record.get("gift_id") == 0 ? 0 : record.get("gift_id"),
                'event_id': currentRecord.data.event_id == null ? 0 : currentRecord.data.event_id,
                'gift_name': record.get("gift_name"),
                'gift_type': record.get("gift_type"),
                'product_id': record.get("product_id"),
                'product_num': record.get("gift_type") == 1 ? record.get("product_num") : 0,
                'gift_ware': record.get("gift_ware"),
                'gift_num': record.get("gift_num"),
                'quantity': record.get("quantity"),
                'amount': record.get("amount")
            });
        }
        var condiType = "";
        switch (promoAmountGift.condition_type) {
            case "1"://品牌
                condiType = Ext.getCmp('p_brand').getValue();
                break;
            case "2"://類別
                condiType = Ext.getCmp('category_id').getValue();//類別內包含的值
                break;
            case "3"://館別
                condiType = Ext.getCmp('p_class').getValue();
                break;
            case "4"://商品
                condiType = Ext.getCmp('p_product_name').getValue();//商品編號&數量,形式
                break;
            case "5"://購物車
                condiType = Ext.getCmp('p_cart').getValue();
                break;
            case "6"://付款方式
                condiType = Ext.getCmp('p_payment').getValue();
                break;
        }
        Ext.Ajax.request({
            url: '/EventPromo/SavePromoAmountGift',
            params: {
                promoAmountGiftStr: Ext.encode(promoAmountGift),
                promoGiftDetailStr: Ext.encode(promoDetail),
                condiType: condiType
            },
            timeout: 360000,
            success: function (response) {
                var res = Ext.decode(response.responseText);
                if (res.success) {
                    Ext.Msg.alert(INFORMATION, SUCCESS);
                    myMask.hide();
                    Ext.getCmp('west-region-container').setDisabled(false);
                    Search();
                    Ext.getCmp('center').getForm().reset();
                    promoDetailStore.removeAll();
                    ac_store.removeAll();
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    myMask.hide();
                }

            },
            failure: function (response, opts) {
                if (response.timedout) {
                    Ext.Msg.alert(INFORMATION, TIME_OUT);
                }
                myMask.hide();
            }
        });
    }
}



//更改活動狀態注：活動的最近修改人不可啟用該活動
function UpdateActive(row) {
    var activeValue = row.data.event_status == 0 ? 1 : 0;
    $.ajax({
        url: "/EventPromo/UpdateActive",
        data: {
            "event_id": row.data.event_id,
            "mo_user": row.data.modify_user,
            "type": row.data.condition_type,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "stop") {
                Ext.Msg.alert(INFORMATION, "管理員不能啟用自己禁用的數據！");
            }
            else if (msg.success == "true") {
                promoGiftListStore.load();
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}


