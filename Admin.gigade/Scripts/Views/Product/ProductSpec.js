/*  
 * 
 * 文件名称：ProductSpec.js 
 * 摘    要：單一商品修改和新增 規格頁面
 * 
 */
var conditionPanel;
var contentPanel
var spec1Rows = 1;
var spec2Rows = 1;
var spec1Store;
var spec2Store;
var spec1Grid;
var spec2Grid;
var specnamePanel = null;
var gridPanel = null;
var isBlur = false;
var typeVal = null;
var conditionViewport;
var isLoad = false;
var isModify = false;
var isChange = false;
var spec_title_1 = '';
var spec_title_2 = '';
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
var myMask = null;
var spec1Init = '';
var spec2Init = '';
var spec1cellEditing = null;
var spec2cellEditing = null;
var asyncResult = true;

Ext.onReady(function () {
    initPanel();
});

var spectypeStore = Ext.create('Ext.data.Store', {
    fields: ['parameterCode', 'parameterName'],
    proxy: {
        type: 'ajax',
        url: '/Product/getCateType',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

/************** 規格 Model Store **************/
Ext.define('GIGADE.SPEC', {
    extend: 'Ext.data.Model',
    fields: [
            { name: 'spec_id', type: 'int' },
            { name: 'product_id', type: 'int' },
            { name: 'spec_type', type: 'int' },
            { name: 'spec_name', type: 'string' },
            { name: 'spec_sort', type: 'int' },
            { name: 'spec_status', type: 'int' }
    ]
});

spec1Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SPEC',
    proxy: {
        type: 'ajax',
        url: '/Product/spec1TempQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

/******************** Spec2 ***********************/
spec2Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.SPEC',
    proxy: {
        type: 'ajax',
        url: '/Product/spec2TempQuery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

/************** 初始化界面 **************/
function initPanel() {
    if (!PRODUCT_ID) {
        PRODUCT_ID = window.parent.GetProductId();
    }
    OLD_PRODUCT_ID = window.parent.GetCopyProductId();

    Ext.Ajax.request({
        url: '/Product/QueryProduct',
        method: 'POST',
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText != null && resText.data != null) {
                typeVal = resText.data.Product_Spec;
                if (typeVal == 1) {
                    isLoad = true;
                    isChange = true;
                    typeVal = 1;
                    spec_title_1 = resText.data.Spec_Title_1;
                    createContentPanel(1);
                }
                else if (typeVal == 2) {
                    isLoad = true;
                    isChange = true;
                    typeVal = 2;
                    spec_title_1 = resText.data.Spec_Title_1;
                    spec_title_2 = resText.data.Spec_Title_2;
                    createContentPanel(2);
                }
                else {
                    if (PRODUCT_ID != '') {
                        typeVal = 0;
                        isLoad = true;
                        Ext.create('Ext.panel.Panel', {
                            padding: '20 0 0 20',
                            border: false,
                            renderTo: Ext.getBody(),
                            items: [{
                                xtype: 'displayfield',
                                labelWidth: 65,
                                fieldLabel: SPEC_TYPE,
                                value: SPEC_NULL
                            }],
                            listeners: {
                                afterrender: function () {
                                    window.parent.setMoveEnable(true);
                                }
                            }
                        });
                    }
                    else {
                        createConditionPanel(resText.data.Product_Spec);
                    }
                }
            }
            else {
                createConditionPanel(null);
            }
        },
        failure: function (response, opts) {
            return false;
        }
    });
}

function createConditionPanel(spectype) {
    conditionPanel = Ext.create('Ext.form.Panel', {
        id: 'conditionPanel',
        title: '',
        padding: '5 0 0 10',
        border: false,
        items: [{
            layout: 'hbox',
            border: false,
            items: [{
                xtype: 'combobox',
                id: 'comboSpecType',
                colName: 'comboSpecType',
                hidden: true,
                labelWidth: 65,
                editable: false,
                allowBlank: false,
                margin: '0 3 0 0',
                fieldLabel: SPEC_TYPE + '<span style="color:red;">*</span>',
                store: spectypeStore,
                queryMode: 'local',
                forceSelection: true,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                listeners: {
                    select: function (comb, record) {
                        typeVal = comb.value;
                        function clear() {
                            Ext.getCmp('numSpec1').setValue('');
                            Ext.getCmp('numSpec2').setValue('');
                            Ext.getCmp('disTitle').hide();
                            Ext.getCmp('numSpec1').hide();
                            Ext.getCmp('disX').hide();
                            Ext.getCmp('numSpec2').hide();
                        }

                        var val = record[0].data.parameterCode;
                        if (val == 0) {
                            clear();

                        }
                        if (val == 1) {
                            clear();
                            Ext.getCmp('disTitle').show();
                            Ext.getCmp('numSpec1').show();
                        }
                        if (val == 2) {
                            clear();
                            Ext.getCmp('disTitle').show();
                            Ext.getCmp('numSpec1').show();
                            Ext.getCmp('disX').show();
                            Ext.getCmp('numSpec2').show();
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                id: 'disTitle',
                fieldLabel: SPEC_NUM + '<span style="color:red;">*</span>',
                // colName: 'disTitle',
                hidden: true,
                labelWidth: 65
            }, {
                xtype: 'numberfield',
                decimalPrecision: 0,
                id: 'numSpec1',
                //  colName: 'numSpec1',
                allowBlank: false,
                hidden: true,
                width: 50,
                minValue: 1,
                listeners: {
                    spin: function (numfield) {
                        numfield.focus(false, true);
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            if (conditionPanel.getForm().isValid()) {
                                blurToGrid();
                            }
                        }
                    }
                }
            }, {
                xtype: 'displayfield',
                id: 'disX',
                //  colName: 'disX',
                value: 'X',
                hidden: true,
                margin: '0 5 0 5'
            }, {
                xtype: 'numberfield',
                decimalPrecision: 0,
                id: 'numSpec2',
                //  colName: 'numSpec2',
                hidden: true,
                width: 50,
                minValue: 1,
                listeners: {
                    spin: function (numfield) {
                        numfield.focus(false, true);
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            if (conditionPanel.getForm().isValid()) {
                                blurToGrid();
                            }
                        }
                    }
                }
            }]
        }, {
            xtype: 'panel',
            id: 'disNotice',
            colName: 'disNotice',
            hidden: true,
            border: false,
            padding: '15 0 0 0',
            html: '<span style="color:red;font-weight:bold;font-size:12px;font-family:雅黑;float:left">' + NOTICE_SPEC_CANNOTMODIFY + '</span>'
        }]
    });

    spectypeStore.load({
        callback: function (records, operation, success) {
            if (spectype != null) {
                Ext.getCmp('comboSpecType').setValue(spectype);
            }
        }
    });

    conditionViewport = Ext.create('Ext.container.Viewport', {
        id: 'conditionViewport',
        items: [conditionPanel],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            beforerender: function (view) {
                window.parent.updateAuth(view, 'colName');
            }
        }
    });

    if (!Ext.getCmp('comboSpecType').isHidden()) {
        Ext.getCmp('disNotice').show();
    }
    else {
        Ext.getCmp('disNotice').hide();
    }
}

function blurToGrid() {
    typeVal = Ext.getCmp('comboSpecType').getValue();
    var num1Value = Ext.getCmp('numSpec1').getValue();
    var num2Value = Ext.getCmp('numSpec2').getValue();

    if (typeVal == 1 && num1Value == null) {
        return;
    }

    if (typeVal == 2 && (num1Value == null || num2Value == null)) {
        return;
    }
    spec1Rows = num1Value;
    spec2Rows = num2Value;

    conditionViewport.destroy();
    createContentPanel(typeVal);
}

function destroyPanel() {
    if (conditionPanel != null) {
        conditionPanel.destroy();
        conditionPanel = null;
    }
    if (specnamePanel != null) {
        specnamePanel.destroy();
        specnamePanel = null;
    }
    if (gridPanel != null) {
        gridPanel.destroy();
        gridPanel = null;
    }
    if (spec1Grid != null) {
        spec1Grid.destroy();
        spec1Grid = null;
    }
    if (spec2Grid != null) {
        spec2Grid.destroy();
        spec2Grid = null;
    }
    spec1Store.removeAll();
    spec2Store.removeAll();
}

function createContentPanel(typeVal) {
    if (isBlur && specnamePanel != null) {
        return;
    }
    destroyPanel();

    /**************** 規格名稱 Panel ****************/
    specnamePanel = Ext.create('Ext.form.Panel', {
        id: 'specnamePanel',
        title: '',
        fieldDefaults: {
            msgTarget: 'side'
        },
        border: false,
        items: [{
            xtype: 'button',
            text: CHANGE_SPECTYPE,
            margin: '0 0 10 0',
            hidden: PRODUCT_ID != '' ? true : false,
            listeners: {
                viewready: function (panel) {
                    window.parent.updateAuth(panel, 'colName');
                }
            },
            handler: function () {
                Ext.Msg.confirm(NOTICE, CNFIRM_DELETE, function (btn) {
                    if (btn == 'yes') {
                        isModify = true;
                        spec1Init = '';
                        async: false,
                        spec2Init = '';
                        Ext.Ajax.request({
                            url: '/Product/specTempDelete',
                            method: 'POST',
                            params: {
                                OldProductId: OLD_PRODUCT_ID
                            },
                            success: function (response) {
                                destroyPanel();
                                createConditionPanel(null);
                            }
                        });
                    }

                });
            }
        }, {
            xtype: 'textfield',
            id: 'txtSpec1Name',
            colName: 'txtSpec1Name',
            hidden: true,
            allowBlank: false,
            submitValue: false,
            fieldLabel: SPEC_1_NAME + '<span style="color:red;">*</span>',
            labelWidth: 100,
            margin: '0 3 10 0'
        }],
        listeners: {
            afterrender: function (panel) {
                window.parent.updateAuth(panel, 'colName');
            }
        }
    });

    if (typeVal == 2) {
        specnamePanel.add({
            xtype: 'textfield',
            id: 'txtSpec2Name',
            colName: 'txtSpec2Name',
            hidden: true,
            allowBlank: false,
            submitValue: false,
            fieldLabel: SPEC_2_NAME + '<span style="color:red;">*</span>',
            labelWidth: 100,
            margin: '0 3 0 0'

        });
    }

    /**************** 規格內容 Grid ****************/
    gridPanel = Ext.create('Ext.panel.Panel', {
        id: 'gridPanel',
        title: '',
        padding: '10 0 0 0',
        border: false,
        layout: 'column',
        items: [],
        listeners: {
            afterrender: function (panel) {
                window.parent.updateAuth(panel, 'colName');
            }
        }
    });

    /****************** 列編輯 ******************/
    spec1cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });

    spec2cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1
    });


    /*******************添加/刪除 Store *******************/
    function spec1StoreAdd(rows) {
        for (var i = 0; i < rows; i++) {
            spec1Store.add({
                spec_name: '',
                spec_sort: '0',
                spec_status: '1'
            });
        }
        // spec1cellEditing.startEditByPosition({ row: spec1Store.getCount() - 1, column: 1 });
    }
    function spec1StoreRemove(rowIdx) {
        var data = spec1Store.getAt(rowIdx);
        data.set('spec_name', '');
        data.set('spec_sort', '0');
        data.set('spec_status', '1');
    }
    function spec2StoreAdd(rows) {
        for (var i = 0; i < rows; i++) {
            spec2Store.add({
                spec_name: '',
                spec_sort: '0',
                spec_status: '1'
            });
        }
        // spec2cellEditing.startEditByPosition({ row: spec2Store.getCount() - 1, column: 1 });
    }
    function spec2StoreRemove(rowIdx) {
        var data = spec2Store.getAt(rowIdx);
        data.set('spec_name', '');
        data.set('spec_sort', '0');
        data.set('spec_status', '1');
    }

    /**************** 規格一 Grid ****************/
    var statusStore1 = Ext.create('Ext.data.Store', {
        id: 'statusStore1',
        autoDestroy: true,
        fields: ['parameterCode', 'parameterName'],
        autoLoad: false,
        data: [
         { parameterCode: '1', parameterName: SPEC_SHOW },
         { parameterCode: '0', parameterName: SPEC_HIDE }
        ]
    })

    function statusRenderer1(value, m, r, row, column) {
        var index = statusStore1.find("parameterCode", value);
        var recode = statusStore1.getAt(index);
        //  currentRow = row;
        if (recode) {
            return recode.get("parameterName");
        } else {
            return value;
        }
    }

    var statusStore2 = Ext.create('Ext.data.Store', {
        id: 'statusStore2',
        autoDestroy: true,
        fields: ['parameterCode', 'parameterName'],
        autoLoad: false,
        data: [
         { parameterCode: '1', parameterName: SPEC_SHOW },
         { parameterCode: '0', parameterName: SPEC_HIDE }
        ]
    });

    function statusRenderer2(value, m, r, row, column) {
        var index = statusStore2.find("parameterCode", value);
        var recode = statusStore2.getAt(index);
        //  currentRow = row;
        if (recode) {
            return recode.get("parameterName");
        } else {
            return value;
        }
    }

    spec1Grid = Ext.create('Ext.grid.Panel', {
        id: 'spec1Grid',
        title: SPEC_1,
        store: spec1Store,
        margin: '0 30 0 0',
        width: 380,
        height: 300,
        columns: [{
            header: '', id: 'delSpec1', colName: 'delSpec', hidden: true, menuDisabled: true, width: 40, align: 'center', xtype: 'actioncolumn', items: [{
                icon: '../../../Content/img/icons/cross.gif',
                handler: function (grid, rowIndex, colIndex) {
                    var specId = spec1Store.getAt(rowIndex).get('spec_id');
                    if (specId != 0 && PRODUCT_ID != '') {
                        return;
                    }
                    Ext.Msg.confirm(NOTICE, CONFIRM_DEL, function (btn) {
                        if (btn == "yes") {
                            if (spec1Store.getCount() > 1) {
                                spec1Store.removeAt(rowIndex);
                            }
                            else {
                                spec1StoreRemove(0);
                            }
                        }
                    });
                }
            }]
        },
        { id: 'spec1_name', colName: 'spec_name', hidden: true, menuDisabled: true, width: 150, header: SPEC_CONTENT, dataIndex: 'spec_name', editor: { width: 200, allowBlank: false } },
        { id: 'spec1_sort', colName: 'spec_sort', hidden: true, menuDisabled: true, width: 80, header: SPEC_SORT, dataIndex: 'spec_sort', editor: { width: 100, xtype: 'numberfield', decimalPrecision: 0, minValue: 0, allowBlank: false } },
        {
            id: 'spec1_status', colName: 'spec_status', hidden: true, menuDisabled: true, width: 80, header: SPEC_SHOW, dataIndex: 'spec_status', renderer: statusRenderer1,
            editor: {
                xtype: 'combobox',
                id: 'combStatus1',
                queryMode: 'local',
                displayField: 'parameterName',
                valueField: 'parameterCode',
                store: statusStore1,
                editable: false,
                allowBlank: false
            }
        },
        { id: 'spec1_id', hidden: true, dataIndex: 'spec_id' }],
        tbar: {
            bodyStyle: 'padding:2px 5px;',
            id: 'barSpec1',
            items: [{
                xtype: 'button',
                text: BTN_ADD,
                id: 'btnSpec1Add',
                colName: 'btnSpec1Add',
                hidden: true,
                border: true,
                iconCls: 'icon-add',
                width: 80,
                handler: function () {
                    spec1StoreAdd(1);
                }
            }]
        },
        plugins: [spec1cellEditing],
        listeners: {
            afterrender: function () {
                spec1StoreAdd(spec1Rows);
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    /**************** 規格二 Grid ****************/
    spec2Grid = Ext.create('Ext.grid.Panel', {
        id: 'spec2Grid',
        title: SPEC_2,
        store: spec2Store,
        margin: '0 30 0 0',
        width: 380,
        height: 300,
        columns: [{
            header: '', id: 'delSpec2', colName: 'delSpec', hidden: true, menuDisabled: true, width: 40, align: 'center', xtype: 'actioncolumn', items: [{
                icon: '../../../Content/img/icons/cross.gif',
                handler: function (grid, rowIndex, colIndex) {
                    var specId = spec2Store.getAt(rowIndex).get('spec_id');
                    if (specId != 0 && PRODUCT_ID != '') {
                        return;
                    }
                    Ext.Msg.confirm(NOTICE, CONFIRM_DEL, function (btn) {
                        if (btn == 'yes') {
                            if (spec2Store.getCount() > 1) {
                                spec2Store.removeAt(rowIndex);
                            }
                            else {
                                spec2StoreRemove(0);
                            }
                        }
                    });

                }
            }]
        },
        { id: 'spec2_name', colName: 'spec_name', hidden: true, menuDisabled: true, width: 150, header: SPEC_CONTENT, dataIndex: 'spec_name', editor: { xtype: 'textfield', width: 200, allowBlank: false } },
        { id: 'spec2_sort', colName: 'spec_sort', hidden: true, menuDisabled: true, width: 80, header: SPEC_SORT, dataIndex: 'spec_sort', editor: { width: 100, xtype: 'numberfield', decimalPrecision: 0, minValue: 0 } },
         {
             id: 'spec2_status', colName: 'spec_status', hidden: true, menuDisabled: true, width: 80, header: SPEC_SHOW, dataIndex: 'spec_status', renderer: statusRenderer2,
             editor: {
                 xtype: 'combobox',
                 id: 'combStatus2',
                 queryMode: 'local',
                 displayField: 'parameterName',
                 valueField: 'parameterCode',
                 store: statusStore2,
                 editable: false
             }
         },
         { id: 'spec2_id', hidden: true, dataIndex: 'spec_id' }],
        tbar: {
            id: 'barSpec2',
            bodyStyle: 'padding:2px 5px;',
            defaultType: 'button',
            items: [{
                id: 'btnSpec2Add',
                colName: 'btnSpec2Add',
                text: BTN_ADD,
                hidden: true,
                border: true,
                iconCls: 'icon-add',
                width: 80,
                handler: function () {
                    spec2StoreAdd(1);
                }
            }]
        },
        plugins: [spec2cellEditing],
        listeners: {
            afterrender: function () {
                spec2StoreAdd(spec2Rows);
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    /*********** 添加規格Grid至規格Panel ***********/
    gridPanel.add(spec1Grid);
    if (isLoad && !isModify) {
        Ext.getCmp('txtSpec1Name').setValue(spec_title_1);
        spec1StoreLoad();
    }

    if (typeVal == 2) {
        gridPanel.add(spec2Grid);
        if (isLoad && !isModify) {
            Ext.getCmp('txtSpec2Name').setValue(spec_title_2);
            spec2StoreLoad();
        }

    }

    specContentViewport = Ext.create('Ext.container.Viewport', {
        id: 'specContentViewport',
        items: [specnamePanel, gridPanel],
        padding: '20 0 0 20',
        layout: 'anchor',
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            },
            beforerender: function (view) {

            },
            afterrender: function () {
                window.parent.setMoveEnable(true);
            }
        }
    });

    isBlur = true;
    isLoad = false;
    //isModify = false;
    //    window.parent.setShow(specnamePanel, 'id');
    //    window.parent.setShow(gridPanel, 'id');
    //    window.parent.setShow(gridPanel, 'colName');

    //修改商品規格時可以添加但不能刪除
    if (PRODUCT_ID != '') {
        //Ext.getCmp('delSpec1').hide();
        //Ext.getCmp('btnSpec1Add').setDisabled(true);

        if (typeVal == 2 && spec2Grid != null) {
            //Ext.getCmp('delSpec2').hide();
            //Ext.getCmp('btnSpec2Add').setDisabled(true);
        }
    }
}

function save(functionid) {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: '請稍等...' });
    }
    mask.show();

    //结束列编辑
    if (spec1cellEditing) {
        spec1cellEditing.completeEdit();
    }
    if (spec2cellEditing) {
        spec2cellEditing.completeEdit();
    }

    var obj = Ext.getCmp('comboSpecType');

    if (Ext.getCmp('conditionPanel') != null && Ext.getCmp('conditionPanel').isHidden()) {
        mask.hide();
        return true;
    }

    if (isLoad == false && typeVal == null) {
        obj.markInvalid(INPUT_PLEASE);
        window.parent.setMoveEnable(true);
        mask.hide();
        return false;
    }
    else if (typeVal != 0 && Ext.getCmp('specnamePanel') == null) {
        if (typeVal == 1) {
            Ext.getCmp('numSpec1').markInvalid(INPUT_PLEASE);
        }

        if (typeVal == 2) {
            if (Ext.getCmp('numSpec1').getValue() == '' || Ext.getCmp('numSpec1').getValue() == null) {
                Ext.getCmp('numSpec1').markInvalid(INPUT_PLEASE);
            }
            if (Ext.getCmp('numSpec2').getValue() == '' || Ext.getCmp('numSpec2').getValue() == null) {
                Ext.getCmp('numSpec2').markInvalid(INPUT_PLEASE);
            }
        }
        window.parent.setMoveEnable(true);
        mask.hide();
        return false;
    } else {//添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
        window.parent.setMoveEnable(false);
    }

    if (typeVal == 1 && Ext.getCmp('txtSpec1Name').getValue() == ' ') {
        Ext.getCmp('txtSpec1Name').markInvalid(INPUT_PLEASE);
        window.parent.setMoveEnable(true);
        mask.hide();
        return false;
    }
    if (typeVal == 2 && Ext.getCmp('txtSpec2Name').getValue() == ' ') {
        Ext.getCmp('txtSpec2Name').markInvalid(INPUT_PLEASE);
        window.parent.setMoveEnable(true);
        mask.hide();
        return false;
    }

    var spec1Result = '';
    var spec2Result = '';
    var spec1Name = '';
    var spec2Name = '';

    if (typeVal != 0) {
        var panelform = Ext.getCmp('specnamePanel').getForm();
        if (panelform.isValid()) {
            var isRight = true;
            spec1Result += '[';
            spec2Result += '';
            var msgStr = COMPLETE_PLEASE;
            var re = /}{/g;
            if (spec1Store.data.length == 0) {
                mask.hide();
                Ext.Msg.alert(INFORMATION, msgStr + ' ' + SPEC_1 + CONTENT);
                return;
            }
            for (var i = 0, j = spec1Store.data.length; i < j; i++) {
                var data1 = spec1Store.getAt(i).data;
                if (data1.spec_name == '') {
                    isRight = false;
                    msgStr += ' ' + SPEC_1;
                    break;
                }
                spec1Result += '{spec_id:' + data1.spec_id + ',spec_name:\'' + data1.spec_name + '\',spec_sort:' + data1.spec_sort + ',spec_status:' + data1.spec_status + '}';
            }
            spec1Result = spec1Result.replace(re, "},{");
            spec1Result += ']';

            if (typeVal == 2) {
                if (spec2Store.data.length == 0) {
                    mask.hide();
                    Ext.Msg.alert(INFORMATION, msgStr + ' ' + SPEC_2 + CONTENT);
                    return;
                }
                spec2Result += '[';
                for (var i = 0, j = spec2Store.data.length; i < j; i++) {
                    var data2 = spec2Store.getAt(i).data;
                    if (data2.spec_name == '') {
                        isRight = false;
                        msgStr += ' ' + SPEC_2;
                        break;
                    }
                    spec2Result += '{spec_id:' + data2.spec_id + ',spec_name:\'' + data2.spec_name + '\',spec_sort:' + data2.spec_sort + ',spec_status:' + data2.spec_status + '}';
                }
                spec2Result = spec2Result.replace(re, "},{");
                spec2Result += ']';
            }
            msgStr += ' ' + CONTENT;
            if (!isRight) {
                mask.hide();
                Ext.Msg.alert(NOTICE, msgStr);
                window.parent.setMoveEnable(true);
                return false;
            }
            spec1Name = Ext.htmlEncode(Ext.getCmp('txtSpec1Name').getValue());
            spec2Name = typeVal == 2 ? Ext.htmlEncode(Ext.getCmp('txtSpec2Name').getValue()) : '';
        }
        else {
            window.parent.setMoveEnable(true);
            mask.hide();
            return false;
        }
    }

    if (!functionid) {
        functionid = '';
    }

    Ext.Ajax.request({
        url: '/Product/specTempSave',
        method: 'POST',
        async: window.parent.GetProductId() == '' ? false : true,
        params: {
            'ProductId': PRODUCT_ID,
            OldProductId: OLD_PRODUCT_ID,
            'specInit': typeVal == 1 ? spec1Init : spec1Init + ',' + spec2Init,
            'isLoad': isLoad,
            'isModify': isModify,
            'specType': typeVal,
            'spec1Name': Ext.htmlEncode(spec1Name),
            'spec2Name': Ext.htmlEncode(spec2Name),
            'spec1Result': Ext.htmlEncode(spec1Result),
            'spec2Result': Ext.htmlEncode(spec2Result),
            "function": functionid,
            "batch": window.parent.GetBatchNo()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            mask.hide();
            if (resText.success) {
                //保存成功后重新加載一遍 避免數據重複添加  edit by zhuoqin0830w  2015/09/24
                spec1StoreLoad(); spec2StoreLoad();
                if (PRODUCT_ID != '' && resText.Msg) {
                    Ext.Msg.alert(NOTICE, SUCCESS_MSG);     //Add by wangwei0216w 2014/9/19
                } else {
                    Ext.Msg.alert(NOTICE, SAVE_SUCCESS);
                }
            }
            else {
                Ext.Msg.alert(NOTICE, SAVE_FAIL);
                asyncResult = false;
            }
            window.parent.setMoveEnable(true);
        },
        failure: function (response, opts) {
            mask.hide();
            Ext.Msg.alert(NOTICE, SAVE_FAIL);
            window.parent.setMoveEnable(true);
            asyncResult = false;
        }
    });
    return asyncResult;
}

function spec1StoreLoad() {
    spec1Store.load({
        params: { 'ProductId': PRODUCT_ID, OldProductId: OLD_PRODUCT_ID },
        callback: function (records, operation, success) {
            if (records && records.length > 0) {
                spec1Init = '';
                for (var i = 0, j = records.length; i < j; i++) {
                    if (i > 0) {
                        spec1Init += ',';
                    }
                    spec1Init += records[i].data.spec_id;
                }
            }
        }
    });
}

function spec2StoreLoad() {
    spec2Store.load({
        params: { 'ProductId': PRODUCT_ID, OldProductId: OLD_PRODUCT_ID },
        callback: function (records, operation, success) {
            if (records && records.length > 0) {
                spec2Init = '';
                for (var i = 0, j = records.length; i < j; i++) {
                    if (i > 0) {
                        spec2Init += ',';
                    }
                    spec2Init += records[i].data.spec_id;
                }
            }
        }
    });
}