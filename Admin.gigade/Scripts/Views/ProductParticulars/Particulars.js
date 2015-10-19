/*****************************************************************************/
Ext.Loader.setConfig({
    enabled: true
});
//Ext.Loader.setPath('Ext.ux', '../../../Scripts/Ext4.0/ux');
Ext.Loader.setPath('Ext.ux', '../../../Scripts/Views/ProductParticulars');
Ext.require([
    'Ext.grid.*',
    'Ext.data.*',
    'Ext.util.*',
    'Ext.state.*',
    'Ext.form.*',
    'Ext.ux.CheckColumn'
]);

//定義商品細項Model&Store
Ext.define('gigade.Particulars', {
    extend: 'Ext.data.Model',
    idProperty: 'Item_id',
    fields: [{ name: "Product_id", type: "string" },
        { name: "Item_id", type: "string" },
        { name: "Product_name", type: "string" },
        { name: "Prod_sz", type: "string" },
        { name: "Pend_del_bool", type: "boolean" },
        { name: "Cde_dt_shp", type: "int" },
        { name: "Pwy_dte_ctl_bool", type: "boolean" },
        { name: "Cde_dt_incr" },//去掉type屬性 以便于 在列編輯中插入combobox  edit by zhuoqin0830w 2015/05/19
        { name: "Cde_dt_var", type: "int" },
        { name: "Hzd_ind", type: "string" },
        { name: "Cse_wid", type: "float" },
        { name: "Cse_wgt", type: "float" },
        { name: "Cse_unit", type: "int" },
        { name: "Cse_len", type: "float" },
        { name: "Cse_hgt", type: "float" },
        { name: "Unit_ship_cse", type: "int" },
        { name: "Inner_pack_wid", type: "float" },
        { name: "Inner_pack_wgt", type: "float" },
        { name: "Inner_pack_unit", type: "int" },
        { name: "Inner_pack_len", type: "float" },
        { name: "Inner_pack_hgt", type: "float" },
        { name: "Spec_name", type: "string" }]
});

var particularsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: false,
    model: 'gigade.Particulars',
    proxy: {
        type: 'ajax',
        url: '/ProductParticulars/QueryParticulars',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

var searchConditionStore = Ext.create('Ext.data.Store', {
    fields: ['conditionId', 'conditions'],
    data: [
        { "conditionId": "1", "conditions": PRODUCT_ID },//商品ID
        { "conditionId": "2", "conditions": PRODUCT_CHILD_ID },//商品子項ID
        { "conditionId": "3", "conditions": BRAND }//品牌
    ]
});

//品牌store
var brandStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: "brand_id", type: "string" },
        { name: "brand_name", type: "string" }],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Brand/QueryBrand",
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

//行編輯
//var rowEditing = Ext.create('Ext.grid.plugin.RowEditing', {
//    clicksToMoveEditor: 1,
//    autoCancel: false,
//    errorSummary: false
//});

var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1,
    listeners: {
        beforeedit: function (e) { currentRow = e.rowIdx; }
    }
});

function search() {
    Ext.getCmp('condition').setValue(Ext.getCmp('condition').getValue().replace(/\s+/g, ','));
    var comb = Ext.getCmp('searchCondition');
    var comc = Ext.getCmp('condition');
    if (!comb.isValid() && !comc.isValid()) {
        return;
    }
    particularsStore.removeAll();
    var condition = Ext.getCmp('searchCondition').getValue();
    var value = condition == 3 ? Ext.getCmp('brand_id').getValue() : Ext.getCmp('condition').getValue();
    particularsStore.load({
        params: { ids: value, condition: condition }
    });
}

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("particulars").down('#plyxkz').setDisabled(selections.length == 0);
            Ext.getCmp("particulars").down('#plddsc').setDisabled(selections.length == 0);
        }
    }
});

var currentRow = 0;
//定義模型  add by zhuoqin0830w 2015/05/19
Ext.define('GIGADE.XMLStore', {
    extend: 'Ext.data.Model',
    idProperty: 'particularsName',
    fields: [{ type: 'int', name: 'Rowid' },
        { type: 'string', name: 'particularsName' },
        { type: 'int', name: 'particularsValid' },
        { type: 'int', name: 'particularsCollect' },
        { type: 'int', name: 'particularsCome' },
        { type: 'int', name: 'oldCollect' },
        { type: 'int', name: 'oldCome' }]
});

//加載Grid數據源  add by zhuoqin0830w 2015/05/19
var XMLStore = Ext.create('Ext.data.Store', {
    autoLoad: true,
    model: 'GIGADE.XMLStore',
    proxy: {
        type: 'ajax',
        url: '/ProductParticulars/QueryParticularsSrc',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    sorters: [{
        property: 'particularsValid',
        direction: 'DESC'
    }]
});

var cellEditingByXML = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

///****************  驗證（Vtype）  *******************/
////商品編號
//Ext.apply(Ext.form.field.VTypes, {
//    regxPNo: function (val, field) {
//        return /^\d{5,}$/.test(val)
//    },
//    regxPNoText: '請輸入有效的ID'
//});

Ext.onReady(function () {
    Ext.grid.RowEditor.prototype.saveBtnText = SAVE;//保存
    Ext.grid.RowEditor.prototype.cancelBtnText = CANCEL;//取消

    //定義商品細項列表
    var particulars = Ext.create('Ext.grid.Panel', {
        id: 'particulars',
        store: particularsStore,
        //width: 1650,
        columnLines: true,
        //frame: true,
        plugins: [cellEditing],
        selModel: sm,
        tbar: [{
            id: 'plyxkz',
            xtype: 'button',
            text: QUANTITY_MANIPULATE_EFFECTIVE_CONTROL_TIME_LIMIT,//批量操作有效控制期
            disabled: true,
            enableToggle: true,
            listeners: {
                click: function () {
                    var row = Ext.getCmp("particulars").getSelectionModel().getSelection();
                    for (var i = 0; i < row.length; i++) {
                        row[i].set("Pwy_dte_ctl_bool", this.pressed);
                    }
                }
            }
        }, {
            id: 'plddsc',
            xtype: 'button',
            text: QUANTITY_MANIPULATE_WAIT_DELETE,//批量操作等待刪除
            disabled: true,
            enableToggle: true,
            listeners: {
                click: function () {
                    var row = Ext.getCmp("particulars").getSelectionModel().getSelection();
                    for (var i = 0; i < row.length; i++) {
                        row[i].set("Pend_del_bool", this.pressed);
                    }
                }
            }
        }, {// add by  zhuoqin0830w  添加 保存期限對照表  2015/05/19
            id: 'comparison',
            colName: 'comparison',
            hidden: true,
            xtype: 'button',
            text: SAVE_TIME_LIMIT_CONTRAST_INFO,//保存期限對照表
            border: true,
            listeners: {
                click: showParticulars
            }
        }],
        columns: [
        { header: PRODUCT_ID, dataIndex: 'Product_id', width: 68, align: 'center', menuDisabled: true, sortable: false },//商品ID
        { header: PRODUCT_CHILD_ID, dataIndex: 'Item_id', width: 68, align: 'center', menuDisabled: true, sortable: false },//商品子項ID
        {
            header: PRODUCT_NAME, dataIndex: 'Product_name', width: 220, align: 'center', menuDisabled: true, sortable: false, flex: 1//商品名稱
            /*,
            renderer: function (value, cellmeta, record) {
                return value + (record.data.Prod_sz ? ' (' + record.data.Prod_sz + ')' : '');
            }*/
        },
        { header: PRODUCT_SPEC, dataIndex: 'Spec_name', width: 60, align: 'center', menuDisabled: true, sortable: false },//規格
        // edit  by  zhuoqin0830w  2015/05/19  將保存期限 提前  並進行 修改
        {
            header: SAVE_TIME_LIMIT, dataIndex: 'Cde_dt_incr', width: 65, align: 'center',//保存期限
            editor: {
                xtype: 'combobox',
                listConfig: { loadMask: false },
                editable: false,
                triggerAction: 'all',
                queryMode: 'local',
                store: XMLStore,
                displayField: 'particularsValid',
                valueField: 'particularsValid',
                listeners: {
                    select: function (combo, record) {
                        particularsStore.getAt(currentRow).set("Cde_dt_shp", record[0].data.particularsCome);
                        particularsStore.getAt(currentRow).set("Cde_dt_var", record[0].data.particularsCollect);
                        //當保存期限被選擇的時候 自動勾選有效期控制  add by zhuoqin0830w  2015/10/19
                        particularsStore.getAt(currentRow).set("Pwy_dte_ctl_bool", true);
                    }
                }
            }, menuDisabled: true, sortable: false
        },
        { header: ALLOW_SELL_DAY, dataIndex: 'Cde_dt_shp', width: 60, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 1 }, menuDisabled: true, sortable: false },//允出天數
        { header: ALLOW_PURCHASE_DAY, dataIndex: 'Cde_dt_var', width: 60, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 1 }, menuDisabled: true, sortable: false },//允收天數

        { header: EASY_BREAKDOWN_LV, dataIndex: 'Hzd_ind', width: 75, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 1, maxValue: 9 }, menuDisabled: true, sortable: false },//易損壞等級
        { header: PRODUCT_OP, dataIndex: 'Unit_ship_cse', width: 75, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, minValue: 1 }, menuDisabled: true, sortable: false },//商品OP
        {
            text: PRODUCT_INSIDE, menuDisabled: true, sortable: false, columns: [//內装
                    {//內装寬度
                        header: PRODUCT_INSIDE_WIDTH, dataIndex: 'Inner_pack_wid', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                        renderer: function (value, color) {
                            return value + 'cm';
                            color.css = 'background-color: #8EE5EE';
                            return color;
                        }
                    },
                    {//內装重量
                        header: PRODUCT_INSIDE_WEIGHT, dataIndex: 'Inner_pack_wgt', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                        renderer: function (value) {
                            return value + 'kg';
                        }
                    },
                    {//內装長度
                        header: PRODUCT_INSIDE_LENGTH, dataIndex: 'Inner_pack_len', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                        renderer: function (value) {
                            return value + 'cm';
                        }
                    },
                    {//內装高度
                        header: PRODUCT_INSIDE_HEIGHT, dataIndex: 'Inner_pack_hgt', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                        renderer: function (value) {
                            return value + 'cm';
                        }
                    },
                    { header: PRODUCT_INSIDE_UNIT, dataIndex: 'Inner_pack_unit', width: 63, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, decimalPrecision: 0, minValue: 1 }, menuDisabled: true, sortable: false }//內装單位
            ]
        },
        {//外箱
            text: PRODUCT_EXTERIOR, menuDisabled: true, sortable: false, columns: [
                {//外箱寬度
                    header: PRODUCT_EXTERIOR_WIDTH, dataIndex: 'Cse_wid', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                    renderer: function (value) {
                        return value + 'cm';
                    }
                },
                {//外箱重量
                    header: PRODUCT_EXTERIOR_WEIGHT, dataIndex: 'Cse_wgt', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                    renderer: function (value) {
                        return value + 'kg';
                    }
                },
                {//外箱長度
                    header: PRODUCT_EXTERIOR_LENGTH, dataIndex: 'Cse_len', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                    renderer: function (value) {
                        return value + 'cm';
                    }
                },
                {//外箱高度
                    header: PRODUCT_EXTERIOR_HEIGHT, dataIndex: 'Cse_hgt', width: 75, align: 'center', editor: { xtype: 'numberfield', decimalPrecision: 1, allowBlank: false, minValue: 0.1 }, menuDisabled: true, sortable: false,
                    renderer: function (value) {
                        return value + 'cm';
                    }
                },
                { header: PRODUCT_EXTERIOR_UNIT, dataIndex: 'Cse_unit', width: 63, align: 'center', editor: { xtype: 'numberfield', allowBlank: false, decimalPrecision: 0, minValue: 1 }, menuDisabled: true, sortable: false }//外箱單位
            ]
        }, {
            xtype: 'checkcolumn',
            header: EFFECTIVE_CONTROL_TIME_LIMIT,
            menuDisabled: true,
            sortable: false,//有效期控制
            dataIndex: 'Pwy_dte_ctl_bool',
            id: 'Pwy_dte_ctl_bool',
            width: 70,
            //editor: {
            //    xtype: 'checkbox'
            //},
            //避免用戶二次點擊才能生效  edit by zhuoqin0830w  2015/10/19
            listeners: {
                click: function (a, b, c) {
                    if (particularsStore.getAt(c).get("Pwy_dte_ctl_bool")) {
                        particularsStore.getAt(c).set("Pwy_dte_ctl_bool", false);
                    } else {
                        particularsStore.getAt(c).set("Pwy_dte_ctl_bool", true);
                    }
                }
            }
        },
        {
            xtype: 'checkcolumn',
            header: WAIT_DELETE,//等待刪除
            dataIndex: 'Pend_del_bool',
            id: 'Pend_del_bool',
            menuDisabled: true,
            sortable: false,
            width: 60,
            //editor: {
            //    xtype: 'checkbox'
            //},
            //避免用戶二次點擊才能生效  edit by zhuoqin0830w  2015/10/19
            listeners: {
                click: function (a, b, c) {
                    if (particularsStore.getAt(c).get("Pend_del_bool")) {
                        particularsStore.getAt(c).set("Pend_del_bool", false);
                    } else {
                        particularsStore.getAt(c).set("Pend_del_bool", true);
                    }
                }
            }
        }],
        dockedItems: [{
            xtype: 'toolbar',
            layout: 'column',
            dock: 'top',
            items: [{
                xtype: 'combobox',
                id: 'searchCondition',
                displayField: 'conditions',
                valueField: 'conditionId',
                fieldLabel: QUERY_CONDITION,//查詢條件
                editable: false,
                labelWidth: 60,
                store: searchConditionStore,
                allowBlank: false,
                forceSelection: true,
                value: 1,
                listeners: {
                    select: function (combo, records) {
                        var brand = Ext.getCmp('brand_id');
                        var condition = Ext.getCmp('condition');
                        if (records[0].data.conditionId == 3) {
                            brand.show();
                            condition.hide();
                        }
                        else {
                            brand.hide();
                            condition.show();
                        }
                    }
                }
            }, {
                xtype: 'combobox',
                fieldLabel: BRAND,//品牌
                store: brandStore,
                id: 'brand_id',
                labelWidth: 40,
                colName: 'brand_id',
                hidden: true,
                name: 'brand_id',
                displayField: 'brand_name',
                typeAhead: true,
                queryMode: 'local',
                valueField: 'brand_id'
            }, {
                xtype: 'textfield',
                id: 'condition',
                fieldLabel: QUERY_CONTENT,//查詢內容
                width: 300,
                //vtype: 'regxPNo',
                labelWidth: 60,
                allowBlank: false,
                enableKeyEvents: true,
                listeners: {
                    keydown: function (field, e) {
                        if (e.keyCode == 13) {
                            search();
                        }
                    }
                }
            }, {
                xtype: 'button',
                id: 'search',
                text: QUERT,//查詢
                border: 2,
                margin: '0 0 0 15',
                iconCls: 'ui-icon ui-icon-search-2 ',
                handler: search
            }, {
                text: RESET,//重置
                id: 'btn_reset',
                margin: '0 0 0 10',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp("brand_id").setValue("");
                        Ext.getCmp("condition").setValue("");
                    }
                }
            }]
        }, {
            xtype: 'toolbar',
            //layout: 'column',
            dock: 'bottom',
            items: [{
                id: 'eventUpdate',
                name: 'eventUpdate',
                width: 100,
                //margin: '0 0 0 10',
                border: '10 5 3 10',
                xtype: 'button',
                iconCls: 'ui-icon ui-icon-checked',
                text: SAVE,
                listeners: {
                    click: function () {
                        var myMask = new Ext.LoadMask(Ext.getBody(), {
                            msg: 'Loading...'
                        });
                        myMask.show();

                        var particulars = [];
                        var upDataStore = particularsStore.getUpdatedRecords();//僅獲得修改后的行 add by wwei0216w 2015/6/24
                        var sumday = 0;
                        if (!upDataStore.length) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, NON_DATA_EDIT);//沒有數據被修改
                            return;
                        }
                        for (var i = 0, j = upDataStore.length ; i < j; i++) {
                            var record = upDataStore[i];
                            //2015/09/09 guodong1130w增加有效期控制判斷
                            var Cde_dt_shp = record.get("Cde_dt_shp");
                            var Cde_dt_var = record.get("Cde_dt_var");
                            var Cde_dt_incr = record.get("Cde_dt_incr");

                            sumday = Cde_dt_shp + Cde_dt_var + Cde_dt_incr;

                            if (sumday > 0 && record.get("Pwy_dte_ctl_bool") == false) {
                                Ext.Msg.alert(INFORMATION, SAVEWRONGMESSAGE);
                                myMask.hide();
                                return;
                            }

                            //add by zhuoqin0830w  2015/10/12  添加判斷  效期天數、允出天數、允收天數若不能為0
                            if (Cde_dt_shp == 0 || Cde_dt_var == 0 || Cde_dt_incr == 0) {
                                Ext.Msg.alert(INFORMATION, SAVE_TIME_LIMIT_IS_NULL);
                                myMask.hide();
                                return;
                            }

                            particulars.push({
                                "Product_id": record.get("Product_id"),//add by wwei0216w 註:後臺需要product_id作為歷史繼續查詢時的查詢條件 2015/7/6 
                                "item_id": record.get("Item_id"),
                                "pend_del_bool": record.get("Pend_del_bool"),
                                "cde_dt_shp": record.get("Cde_dt_shp"),
                                "pwy_dte_ctl_bool": record.get("Pwy_dte_ctl_bool"),
                                "cde_dt_incr": record.get("Cde_dt_incr"),
                                "cde_dt_var": record.get("Cde_dt_var"),
                                "hzd_ind": record.get("Hzd_ind"),
                                "cse_wid": record.get("Cse_wid"),
                                "cse_wgt": record.get("Cse_wgt"),
                                "cse_unit": record.get("Cse_unit"),
                                "cse_len": record.get("Cse_len"),
                                "cse_hgt": record.get("Cse_hgt"),
                                "unit_ship_cse": record.get("Unit_ship_cse"),
                                "inner_pack_wid": record.get("Inner_pack_wid"),
                                "inner_pack_wgt": record.get("Inner_pack_wgt"),
                                "inner_pack_unit": record.get("Inner_pack_unit"),
                                "inner_pack_len": record.get("Inner_pack_len"),
                                "inner_pack_hgt": record.get("Inner_pack_hgt")
                            });
                        }
                        particulars = JSON.stringify(particulars);
                        if (particulars == "[]") {
                            myMask.hide();
                            return;
                        }
                        Ext.Ajax.request({
                            url: '/ProductParticulars/SaveParticulars',
                            params: {
                                particulars: particulars,
                                eventUpdate: 'eventUpdate'//add by wwei0216w 該頁修改需要添加在歷史記錄記載 2015/6/16
                            },
                            timeout: 360000,
                            success: function (response) {
                                var res = Ext.decode(response.responseText);
                                if (res.success) {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    search();
                                } else {
                                    Ext.Msg.alert(INFORMATION, MANIPULATE_FAILED);//操作失敗
                                }
                                myMask.hide();
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
            }]
        }]
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [particulars],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //particulars.height = document.documentElement.clientHeight - 18;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

//添加 顯示保存期限 對照表 的方法  add by zhuoqin0830w  2015/05/19
function showParticulars() {
    var ShowExpiry = Ext.create('Ext.grid.Panel', {
        id: 'ShowExpiry',
        store: XMLStore,
        autoScroll: true,
        columnLines: true,
        plugins: [cellEditingByXML],
        frame: false,
        columns: [
        { header: THIS_NAME, dataIndex: 'particularsName', width: 70, align: 'center', hidden: true },//名稱
        { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 46, align: 'center' },//序號
        {
            text: EFFECTIVE_DAY, dataIndex: 'particularsValid', width: 120, align: 'center', sortable: false, menuDisabled: true,//有效天數
            editor: {
                xtype: 'numberfield',
                allowBlank: false,
                minValue: 1,
                listeners: {
                    blur: function (a, b) {
                        for (var i = 0, j = XMLStore.totalCount; i < j; i++) {
                            if (a.value == XMLStore.data.items[i].data.particularsValid) {
                                Ext.Msg.alert(INFORMATION, SAME_PARTICULARSVALID);
                                a.setValue("0");
                                return;
                            }
                        }
                    }
                }
            }
        }, {
            text: ALLOW_SELL_DAY, dataIndex: 'particularsCome', colName: 'particularsCome', width: 120, align: 'center', hidden: true, menuDisabled: true, sortable: false,//允出天數
            editor: {
                xtype: 'numberfield',
                allowBlank: false,
                minValue: 1
            }
        },
        {
            text: ALLOW_PURCHASE_DAY, dataIndex: 'particularsCollect', colName: 'particularsCollect', width: 120, align: 'center', hidden: true, menuDisabled: true, sortable: false,//允收天數
            editor: {
                xtype: 'numberfield',
                allowBlank: false,
                minValue: 1
            }
        },
        //參數表編號
        { header: THIS_NAME, dataIndex: 'Rowid', width: 70, align: 'center', hidden: true }],
        listeners: {
            //是否可顯示
            viewready: function () {
                setShow(ShowExpiry, 'colName');
            },
            beforerender: function () {
                ToolAuthorityQuery(function () {
                    setShow(ShowExpiry, 'colName');
                    if ((ShowExpiry.columns[3].hidden && ShowExpiry.columns[4].hidden) || (ShowExpiry.columns[3].hidden || ShowExpiry.columns[4].hidden)) {
                        Ext.getCmp("addTr").setDisabled(true);
                    }
                });
            },
            //是否可編輯
            beforeedit: function (e) {
                var data = XMLStore.data.items[e.rowIdx];
                if (e.colIdx == 2) {
                    if (data.data.particularsName != "") {
                        return false;
                    }
                }
            },
            validateedit: function (editor, e) { }
        },
        tbar: [{
            text: INSERT,//添加
            id: 'addTr',
            colName: 'addTr',
            hidden: true,
            //disabled: true,
            iconCls: 'icon-add',
            handler: addTr
        }, '->', { text: '' }],
        dockedItems: [{
            dock: 'bottom',
            xtype: 'toolbar',
            items: [{
                id: 'SaveXML',
                name: 'SaveXML',
                colName: 'SaveXML',
                hidden: true,
                border: '10 5 3 10',
                xtype: 'button',
                iconCls: 'ui-icon ui-icon-checked',
                text: SAVE,
                listeners: {
                    click: function () {
                        var myMask = new Ext.LoadMask(Ext.getBody(), {
                            msg: 'Loading...'
                        });
                        myMask.show();
                        //獲取修改的Store
                        var updateStore = XMLStore.getUpdatedRecords();
                        //獲取新增的Store
                        var addStore = XMLStore.getNewRecords();
                        //判斷是否存在修改或新增的值
                        if (!updateStore.length && !addStore.length) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, NON_DATA_EDIT);//沒有數據被修改
                            return;
                        }
                        //將需要修改的值插入數據庫
                        var particularsNode = "[";
                        if (updateStore.length) {
                            for (var i = 0, j = updateStore.length; i < j; i++) {
                                var data = updateStore[i];
                                particularsNode += "{Rowid:'" + data.get('Rowid');
                                particularsNode += "',particularsName:'" + data.get('particularsName');
                                particularsNode += "',particularsValid:'" + data.get('particularsValid');
                                particularsNode += "',particularsCollect:'" + data.get('particularsCollect');
                                particularsNode += "',particularsCome:'" + data.get('particularsCome');
                                particularsNode += "',oldCollect:'" + data.get('oldCollect');
                                particularsNode += "',oldCome:'" + data.get('oldCome') + "'}";
                            }
                        }
                        if (addStore.length) {
                            for (var i = 0, j = addStore.length; i < j; i++) {
                                var data = addStore[i];
                                if (data.get('particularsValid') != 0 && data.get('particularsCollect') != 0 && data.get('particularsCome') != 0) {
                                    particularsNode += "{Rowid:'" + data.get('Rowid');
                                    particularsNode += "',particularsName:'" + "null";
                                    particularsNode += "',particularsValid:'" + data.get('particularsValid');
                                    particularsNode += "',particularsCollect:'" + data.get('particularsCollect');
                                    particularsNode += "',particularsCome:'" + data.get('particularsCome') + "'}";
                                } else {
                                    Ext.Msg.alert(INFORMATION, PARAMETER_MUST_GT_0); //提示信息,參數必須大於0
                                    particularsNode = "[";
                                    break;
                                }
                            }
                        }
                        var re = /}{/g;
                        particularsNode = particularsNode.replace(re, "},{");
                        particularsNode += "]";
                        if (particularsNode == "[]") {
                            myMask.hide();
                            return;
                        }
                        Ext.Ajax.request({
                            url: '/ProductParticulars/SaveNode',
                            params: { particularsNode: particularsNode },
                            success: function (response) {
                                var res = Ext.decode(response.responseText);
                                if (res.success) {
                                    Ext.Msg.alert(INFORMATION, MANIPULATE_SUCCESS, function () {//提示信息,操作成功
                                        XMLStore.load();
                                    });
                                    myMask.hide();
                                } else {
                                    Ext.Msg.alert(INFORMATION, MANIPULATE_FAILED);//提示信息,操作失敗
                                    myMask.hide();
                                }
                            }
                        });
                    }
                }
            }, '->', { text: '' }]
        }, {
            dock: 'bottom',
            xtype: 'pagingtoolbar',
            store: XMLStore,
            displayInfo: true,
            displayMsg: SHOW_ALL_COUNT,//显示 {0} - {1} 条，共计 {2} 条
            emptyMsg: NON_DATA//没有数据
        }]
    });

    var ExpiryWin = new Ext.Window({
        title: SAVE_TIME_LIMIT_CONTRAST_INFO,//保存期限對照表
        id: 'ExpiryWin',
        height: document.documentElement.clientHeight * 600 / 783,
        width: 450,
        //closeAction: 'destroy',
        modal: true,
        layout: 'fit',
        items: [ShowExpiry],
        closable: false,
        tools: [{
            type: 'close',
            qtip: '關閉',
            handler: function (event, toolEl, panel) {
                ExpiryWin.destroy();
                XMLStore.load();
            }
        }]
        //closable: true,
        //listeners: {
        //    beforerender: function () { XMLStore.load(); }
        //}
    }).show();
}

//添加Grid行 add by zhuoqin0830w  2015/05/19 
function addTr() {
    XMLStore.add({
        particularsValid: '0',
        particularsCollect: '0',
        particularsCome: '0'
    });
    cellEditingByXML.startEditByPosition({ row: XMLStore.getCount() - 1, column: 1 });
    currentRow = -1;
}