var rigW = 650; //右側的寬度
var theight = 550; //窗口的高度
var topheight = 100; //窗口的高度
var gridheight = 386;
var pageSize = 25;
var promationWin;
var boolClass = false;
var centerNorthPan;
var p_store;

Ext.onReady(function () {
    BrandStore.on('beforeload', function () {
        Ext.apply(BrandStore.proxy.extraParams, {
            isShowGrade: 1,
            class_id: Ext.htmlEncode(Ext.getCmp("class").getValue())
        });
    });
    ShopClassStore.load();
    centerNorthPan = new Ext.form.FormPanel({
        region: 'north',
        width: rigW,
        height: topheight,
        labelAlign: 'right',
        buttonAlign: 'right',
        padding: 10,
        border: 0,
        items: [
            {
                layout: 'column',
                border: false,
                labelSeparator: ':',
                items: [
                    {
                        xtype: 'combobox', //class_id
                        fieldLabel: "館別",
                        editable: false,
                        id: 'class',
                        labelWidth: 100,
                        name: 'class',
                        hiddenName: 'class_id',
                        colName: 'class_id',
                        store: ShopClassStore,
                        displayField: 'class_name',
                        valueField: 'class_id',
                        typeAhead: true,
                        forceSelection: false,
                        emptyText: "請選擇",
                        listeners: {
                            "select": function (combo, record) {
                                var z = Ext.getCmp("brand");
                                z.clearValue();
                                z.setDisabled(false);
                                BrandStore.removeAll();
                                boolClass = true;
                            }
                        }
                    }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                  {
                      xtype: 'combobox', //banner_id
                      fieldLabel: "品牌",
                      editable: true,
                      queryMode: 'local',
                      id: 'brand',
                      labelWidth: 100,
                      name: 'brand',
                      disabled: true,
                      hiddenname: 'brand_id',
                      store: BrandStore,
                      displayField: 'brand_name',
                      valueField: 'brand_id',
                      typeAhead: true,
                      forceSelection: false,
                      emptyText: "請選擇",
                      listeners: {
                          beforequery: function (qe) {
                              if (boolClass) {
                                  delete qe.combo.lastQuery;
                                  BrandStore.load();
                                  boolClass = false;
                              }
                          }
                      }
                  }
                ]
            },
            {
                layout: 'column',
                border: false,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '查詢所有商品',
                        id: 'key',
                        name: 'key',
                        regex: /^(\d+)([,，|]{1}\d+)*(\d+)*$/,
                        regexText: '輸入格式不符合要求',//add by sdy
                        labelWidth: 100,
                        columnWidth: .7
                    },
                    {
                        xtype: 'button',
                        fieldLabel: "商品查詢",
                        name: 'visitdate',
                        text: "商品查詢",
                        margin: '0 0 0 10',
                        columnWidth: .2,
                        handler: Search
                    }
                ]
            }
        ]
    });



    Ext.define('GIGADE.PRODUCT', {
        extend: 'Ext.data.Model',
        fields: [
        { name: 'id', type: 'int' },
        { name: 'product_id', type: 'string' },
        { name: 'brand_id', type: 'string' },
        { name: 'brand_name', type: 'string' },
        { name: 'product_name', type: 'string' },
        { name: 'product_price_list', type: 'int' },
        { name: 'price', type: 'int' },
        { name: 'cost', type: 'int' },
        { name: 'product_freight_set', type: 'string' }
        ]
    });

    p_store = Ext.create('Ext.data.Store', {
        pageSize: pageSize,
        model: 'GIGADE.PRODUCT',
        proxy: {
            type: 'ajax',
            url: '/EventPromo/QueryProList',
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item',
                totalProperty: 'totalCount'
            }
        }
    });


    p_store.on("beforeload", function () {
        Ext.apply(p_store.proxy.extraParams, {
            key: Ext.htmlEncode(Ext.getCmp('key').getValue()),
            brand_id: document.getElementsByName('brand').length == 0 ? '' : Ext.htmlEncode(document.getElementsByName('brand')[0].value),
            class_id: Ext.getCmp('class').getValue() == '' ? '' : Ext.htmlEncode(Ext.getCmp('class').getValue())
        });
    });

    function Search() {
        p_store.removeAll();
        p_store.load({
            callback: function () {
                if (p_store.data.items.length == 0) {
                    Ext.Msg.alert(INFORMATION, "沒有符合條件的數據");
                }
            }
        });
    }

    var t_search_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("searchGrid").down('#t_add').setDisabled(selections.length == 0);
            }
        }
    });
    var t_activity_cm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("activityGrid").down('#t_remove').setDisabled(selections.length == 0);
                Ext.getCmp("activityGrid").down('#t_set').setDisabled(selections.length == 0);

            }
        }
    });

    var searchGrid = new Ext.grid.Panel({
        id: 'searchGrid',
        store: p_store,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: gridheight,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: "商品編號", dataIndex: 'product_id', width: 60 },
            { header: "品牌", dataIndex: 'brand_name' },
            { header: '品牌id', dataIndex: 'brand_id', hidden: true },
            { header: "商品名稱", dataIndex: 'product_name' },
            { header: "運送方式", dataIndex: 'product_freight_set' },
            {
                header: '商品金額', dataIndex: 'price',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 0) {
                        return "<span style='color:Red;'>" + value + "</span>";
                    } else {
                        return value;
                    }
                }
            }, //商品金額為0時，紅色顯示
            { header: '成本', dataIndex: 'cost' }
        ],
        tbar: [
            { xtype: 'button', id: 't_add', text: "新增至活動", iconCls: 'icon-add', disabled: true, handler: function () { onToolAddClick(); } },
        ],
        selModel: t_search_cm,
        bbar: Ext.create('Ext.PagingToolbar', {
            store: p_store,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        })

    });

    var activityGrid = new Ext.grid.Panel({
        id: 'activityGrid',
        store: ac_store,
        region: 'center',
        autoScroll: true,
        border: 0,
        height: gridheight,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: "商品編號", dataIndex: 'product_id', width: 100 },
            { header: "商品名稱", dataIndex: 'product_name', width: 320 },
            {
                header: "參與數量", dataIndex: 'product_num', align: 'center', editor: {
                    xtype: 'numberfield',
                    allowBlank: false,
                    minValue: 0
                }
            }
        ],
        tbar: [
            { xtype: 'button', id: 't_remove', text: "移除", iconCls: 'icon-remove', disabled: true, handler: function () { onToolRemoveClick(); } }
            ,
            { xtype: 'button', id: 't_set', text: "批次設定數量", iconCls: 'icon-remove', disabled: true, handler: function () { onToolSetNumClick(); } }

        ],
        selType: 'cellmodel',
        plugins: [Ext.create('Ext.grid.plugin.CellEditing', { clicksToEdit: 1 })],
        viewConfig: { emptyText: '<span>暫無數據！</span>' },
        selModel: t_activity_cm,
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }

    });

    var centerSouthPan = new Ext.tab.Panel({
        region: 'center',
        autoScroll: true,
        items: [
            {
                title: "篩選商品",
                items: [searchGrid]
            }, {
                title: "活動商品",
                items: [activityGrid]
            }
        ]
    });

    var centerPanel = new Ext.Panel({
        region: 'center',
        items: [centerNorthPan, centerSouthPan]
    }); //頁面佈局

    promationWin = new Ext.Window({
        title: "活動商品設定",
        height: theight,
        width: rigW,
        layout: 'border',
        modal: true,
        constrain: true,
        closeAction: 'hide', //hide
        items: [
            centerPanel
        ],
        listeners: {
            'close': function () {
                var a = "";
                var b = "";
                if (ac_store.getCount() > 0) {
                    for (var i = 0; i < ac_store.getCount() ; i++) {
                        var record = ac_store.getAt(i);
                        a = a + record.get('product_id') + '&' + record.get('product_num') + ',';
                        b = b + record.get('product_id') + ',';
                    }
                    var allproduct = b.substring(0, b.lastIndexOf(','));
                    var allproductid_num = a.substring(0, a.lastIndexOf(','));
                    Ext.getCmp("p_product_id").setValue(allproduct);
                    Ext.getCmp("p_product_name").setValue(allproductid_num);
                }
                else {
                    Ext.getCmp("p_product_id").setValue("");
                    Ext.getCmp("p_product_name").setValue("");
                }
            }
        }
    });
    function onToolRemoveClick() {
        var row = Ext.getCmp("activityGrid").getSelectionModel().getSelection();
        if (row.length < 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        }
        else {
            Ext.MessageBox.confirm("提示", Ext.String.format("確定移除選中的{0}條數據?", row.length), function (btn) {
                if (btn == "yes") {
                    ac_store.remove(row);
                }
                else {
                    return false;
                }
            });
        }
    }

    function isRepeat(pid) {
        var isR = false;
        for (var i = 0; i < ac_store.getCount() ; i++) {
            var record = ac_store.getAt(i);
            if (pid == record.get('product_id')) {
                isR = true;
            }
        }
        return isR;
    }
    function onToolAddClick() {
        var row = Ext.getCmp("searchGrid").getSelectionModel().getSelection();
        var repeatP = "";
        for (var i = 0; i < row.length; i++) {
            var product_id = row[i].get('product_id');
            var product_name = row[i].get('product_name');

            if (!isRepeat(product_id)) {
                ac_store.add({
                    product_id: product_id,
                    product_name: product_name,
                    product_num: 1
                });
            }
            else {

                repeatP += product_id + ",";
            }
        }
        if (repeatP != "") {
            repeatP = repeatP.substr(0, repeatP.length - 1);
            Ext.Msg.alert(INFORMATION, Ext.String.format("該活動中已存在商品{0}", repeatP));
        }
        else {
            Ext.Msg.alert(INFORMATION, SUCCESS);
        }
        p_store.remove(row);
    }
    function onToolSetNumClick() {
        var row = Ext.getCmp("activityGrid").getSelectionModel().getSelection();
        if (row.length < 0) {
            Ext.Msg.alert(INFORMATION, NO_SELECTION);
        }
        else {
            var editNumFrm = Ext.create('Ext.form.Panel', {
                id: 'editNumFrm',
                frame: true,
                plain: true,
                layout: 'anchor',
                defaults: { anchor: "95%", msgTarget: "side" },
                items: [{
                    xtype: 'numberfield',
                    fieldLabel: "參與數量",
                    id: "n_set",
                    allowBlank: false,
                    minValue: 0
                }
                ],
                buttons: [
                    {
                        text: SAVE,
                        formBind: true,
                        disabled: true,
                        handler: function () {
                            var form = this.up('form').getForm();
                            if (form.isValid()) {
                                for (var i = 0; i < row.length; i++) {
                                    row[i].set('product_num', Ext.getCmp("n_set").getValue());
                                }
                                editNumWin.close();
                            }
                        }
                    }
                ]
            });

            var editNumWin = Ext.create('Ext.window.Window', {
                title: "設定數量",
                id: 'editNumWin',
                iconCls: 'icon-user-auth',
                width: 300,
                height: 120,
                layout: 'fit',
                items: [editNumFrm],
                constrain: true, //束縛窗口在框架內
                closeAction: 'destroy',
                modal: true,
                resizable: false,
                bodyStyle: 'padding:5px 5px 5px 5px',
                closable: false,
                tools: [
                    {
                        type: 'close',
                        qtip: '是否關閉',
                        handler: function (event, toolEl, panel) {
                            Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                                if (btn == "yes") {
                                    Ext.getCmp('editNumWin').destroy();
                                } else {
                                    return false;
                                }
                            });
                        }
                    }
                ],
                listeners: {
                    'show': function () {
                        editNumFrm.getForm().reset(); //如果是編輯的話
                    }
                }
            });
            editNumWin.show();

        }
    }
});

function PromationMationShow() {
    p_store.removeAll();
    centerNorthPan.form.reset();
    promationWin.show();
}