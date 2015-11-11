var pageSize = 30;
var schedule_id = 0;//排程id
var itemId = 0;
Ext.define('GIGADE.Tier', {
    extend: 'Ext.data.Model',
    fields: [{ name: 'schedule_id', type: 'int' },
        { name: 'schedule_name', type: 'string' },
        { name: 'type', type: 'int' },
        { name: 'execute_type', type: 'string' },
        { name: 'day_type', type: 'int' },
        { name: 'month_type', type: 'int' },
        { name: 'date_value', type: 'int' },
        { name: 'repeat_count', type: 'int' },
        { name: 'repeat_hours', type: 'int' },
        { name: 'time_type', type: 'int' },
        { name: 'week_day', type: 'string' },
        { name: 'start_time', type: 'string' },
        { name: 'end_time', type: 'string' },
        { name: 'duration_start', type: 'string' },
        { name: 'duration_end', type: 'string' },
        { name: 'desc', type: 'string' },
        { name: 'create_user', type: 'int' },
        { name: 'create_user_name', type: 'string' },
        { name: 'create_date', type: 'string' },
        { name: 'execute_days', type: 'string' },
        { name: 'trigger_time', type: 'string' }]
});

var tierStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Tier',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/ProductTier/GetTiers',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});

var searchtypeStore = Ext.create('Ext.data.Store', {
    fields: ["ParameterCode", "parameterName"],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryParaByXml?paraType=tiresSearchType',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});

///獲取type 的類型信息
var typeStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName', 'TopValue'],
    proxy: {
        type: 'ajax',
        url: '/ProductTier/GetRelevantType?type=ScheduleType',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    autoLoad: true
});

///獲取key 的類型信息
var keyStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    proxy: {
        type: 'ajax',
        url: '/ProductTier/GetRelevantType?type=Schedule_Key',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    autoLoad: true
});

///獲取value信息
var valueStore = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductTier/GetValueInfo',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

//對應Grid
var relevantStore = Ext.create('Ext.data.Store', {
    fields: [{ name: 'id', type: 'int' },
        { name: 'schedule_Id', type: 'int' },
        { name: 'item_name', type: 'string' },
        { name: 'type', type: 'int' },
        { name: 'key1', type: 'int' },
        { name: 'key2', type: 'int' },
        { name: 'key3', type: 'int' },
        { name: 'value1', type: 'string' },
        { name: 'value2', type: 'string' },
        { name: 'value3', type: 'string' },
        { name: 'schedule_name', type: 'string' },
        { name: 'desc', type: 'string' },
        { name: 'tabType', type: 'string' },
        { name: 'keyStr', type: 'string' },
        { name: 'valueStr', type: 'string' }],
    proxy: {
        type: 'ajax',
        idProperty: 'schedule_Id',
        url: '/ProductTier/relevantStore',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    },
    autoLoad: false
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("tierGrid").down('#remove').setDisabled(selections.length == 0);
            Ext.getCmp("tierGrid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("tierGrid").down('#relevantSet').setDisabled(selections.length == 0);
        }
    }
});

var searchform = Ext.create('Ext.form.Panel', {
    id: 'dockedItem',
    //xtype: 'toolbar',
    layout: 'column',
    border: true,
    frame: true,
    dock: 'top',
    items: [{
        xtype: 'textfield',
        fieldLabel: ID_CODE,//排程編號
        id: 'schedule_id',
        //name: 'schedule_id',
        labelWidth: 60
    }, {
        xtype: 'textfield',
        fieldLabel: TIER_NAME,//排程名稱
        id: 's_schedule_name',
        margin: '0 0 0 10',
        labelWidth: 60
    }, {
        xtype: 'combobox',
        fieldLabel: TIME_CONDITION,//時間條件
        store: searchtypeStore,
        labelWidth: 60,
        margin: '0 0 0 10',
        id: 'search_date_type',
        name: 'search_date_type',
        displayField: 'parameterName',
        valueField: 'ParameterCode',
        editable: false,
        queryMode: 'local'
    }, {
        xtype: 'datefield',
        id: 'time_start',
        name: 'time_start',
        margin: '0 5px',
        editable: false,
        listeners: {    //add by mingwei0727w 2015/09/14 限制時間輸入範圍
            change: function () {
                Ext.getCmp("time_end").setMinValue(this.getValue());
            }
        }
    }, {
        xtype: 'displayfield',
        value: '~'
    }, {
        xtype: 'datefield',
        id: 'time_end',
        name: 'time_end',
        margin: '0 5px',
        editable: false,
        listeners: {
            change: function () {
                Ext.getCmp("time_start").setMaxValue(this.getValue());
            }
        }
    }, {
        xtype: 'button',
        text: QUERY,//查詢
        id: 'btn_search',
        handler: Search,
        margin: '0 0 0 10',
        iconCls: 'ui-icon ui-icon-search-2'
    }, {
        xtype: 'button',
        text: RESET,//重置
        id: 'btn_reset',
        margin: '0 0 0 10',
        iconCls: 'ui-icon ui-icon-reset',
        listeners: {
            click: function () {
                Ext.getCmp("schedule_id").setValue("");
                Ext.getCmp("s_schedule_name").setValue("");
                Ext.getCmp("search_date_type").setValue("");
                Ext.getCmp("time_start").setValue("");
                Ext.getCmp("time_end").setValue("");
            }
        }
    }]

})

Ext.onReady(function () {
    //document.body.onkeydown = function () {
    //    if (e.which == 13) {
    //        $("#btn_search").click();
    //    }
    //};
    ///edit by wwei0216w  /*該版本兼容火狐之前不兼容*/
    document.onkeydown = function (event) {
        e = event ? event : (window.event ? window.event : null);
        if (e.keyCode == 13) {
            //执行的方法 
            $("#btn_search").click();
        }
    }

    var tierGrid = Ext.create('Ext.grid.Panel', {
        id: 'tierGrid',
        store: tierStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: false,
        dockedItems: [searchform],
        tbar: [
           { xtype: 'button', id: 'add', text: ADD, iconCls: 'ui-icon ui-icon-add', handler: onAddClick },//添加
           { xtype: 'button', id: 'edit', text: EDIT, iconCls: 'ui-icon ui-icon-pencil', disabled: true, handler: onEditClick },//編輯
           { xtype: 'button', id: 'remove', text: REMOVE, iconCls: 'ui-icon ui-icon-delete', disabled: true, handler: onRemoveClick },//移除
           { xtype: 'button', id: 'relevantSet', text: PARALLELISM_CONFIG, iconCls: 'ui-icon ui-icon-connect', disabled: true, handler: onRelevantClick }],//對應設置
        columns: [
            { xtype: 'rownumberer', width: 35, align: 'center', menuDisabled: true, sortable: false },
            {
                header: RELEVANCE_DETAILS, xtype: 'actioncolumn', width: 55, align: 'center',//關係詳情
                items: [{
                    icon: '/Content/img/icons/application_view_list.png',
                    iconCls: 'icon-cursor',
                    tooltip: DETAILS_INFORMATION,//詳細信息
                    handler: function (grid, rowIndex, colIndex) {
                        var rec = grid.getStore().getAt(rowIndex);
                        showRelationDetail(rec, this);
                    }
                }]
            },
            { header: SCHEDULE_CODE, dataIndex: 'schedule_id', width: 60, align: 'left', menuDisabled: true, sortable: false },//排程編號
            { header: SCHEDULE_NAME, dataIndex: 'schedule_name', width: 200, align: 'left', menuDisabled: true, sortable: false },//排程名稱
            {
                header: SCHEDULE_TYPE, dataIndex: 'type', width: 99, align: 'left', menuDisabled: true, sortable: false, renderer: function (value) {//排程類型
                    if (value == '1') {
                        return EXECUTE_ONCE;
                    } if (value == '2') {
                        return EXECUTE_REPEAT;
                    } if (value == '3') {
                        return EXECUTE_IRREGULAR;
                    }
                }
            },
            {
                header: EXECUTE_TYPE, dataIndex: 'execute_type', width: 99, align: 'left', menuDisabled: true, sortable: false, renderer: function (value) {//執行類型
                    if (value == '2D') {
                        return EVERYDAY;//每日
                    } else if (value == '2W') {
                        return EVERYWEEK;//每週
                    } else if (value == '2M') {
                        return EVERYMONTH;//每月
                    } else {
                        return '--';
                    }
                }
            },
            { header: EDIT_TIME, dataIndex: 'create_date', width: 160, align: 'left', renderer: Ext.util.Format.dateRenderer('Y-m-d'), menuDisabled: true, sortable: false },//修改時間 2015.08.24
            { header: END_EDIT_USER, dataIndex: 'create_user_name', width: 80, align: 'left', menuDisabled: true, sortable: false },//最后創建人 2015.08.24
            { header: BEGIN_TIME, hidden: true, dataIndex: 'duration_start', width: 120, align: 'left', renderer: Ext.util.Format.dateRenderer('Y/m/d'), menuDisabled: true, sortable: false },//開始時間
            { header: END_TIME, hidden: true, dataIndex: 'duration_end', width: 120, align: 'left', renderer: Ext.util.Format.dateRenderer('Y/m/d'), menuDisabled: true, sortable: false },//結束時間
            { header: DESCRIBE, dataIndex: 'desc', width: 500, align: 'left', menuDisabled: true, sortable: false, flex: 1 }],//描述
        listeners: {
            //itemdblclick: onDoubleClick,
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'fit',
        items: [tierGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                tierGrid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });
    tierStore.load();
})

RelationDetail = Ext.create('Ext.window.Window', {
    title: RELEVANCE_DETAILS,//關係詳情
    width: 650,
    height: 450,
    layout: 'anchor',
    items: [{
        xtype: 'panel',
        height: 416,
        items: [gxGrid]
    }],
    closeAction: 'hide',
    modal: true,
    resizable: true,
    autoScroll: true
})

function Search() {
    Ext.getCmp('schedule_id').setValue(Ext.getCmp('schedule_id').getValue().replace(/\s+/g, ','));
    //tierStore.removeAll();
    tierStore.load({
        params: {
            schedule_id: Ext.getCmp("schedule_id").getValue(),
            schedule_name: Ext.getCmp("s_schedule_name").getValue(),
            SearchType: Ext.getCmp('search_date_type').getValue(),
            start: Ext.getCmp('time_start').getValue(),
            end: Ext.getCmp('time_end').getValue()
        }
    });
}

function showRelationDetail(rec) {
    Ext.getCmp('gxGrid').show();
    gxGridStore.load({
        params: {
            schedule_id: rec.data.schedule_id
        }
    });
    RelationDetail.show();
}

function onAddClick() {
    addPc.show();
    irregulartimeStore.removeAll();
    pcFrm.getForm().reset();
    Ext.getCmp('gxGrid').hide();
    Ext.getCmp('mst').setText("");
    Ext.getCmp("cs_time").setMaxValue('9999/12/31');
    Ext.getCmp("ce_time").setMinValue('0001/01/01');
    Ext.getCmp('irrms').setText("");
    Ext.getCmp('ms').setText("");
    Ext.getCmp('btnSave').show();
}

function onEditClick() {
    var startTime = Ext.getCmp("cs_time").getValue();
    var endTime = Ext.getCmp("ce_time").getValue();
    Ext.getCmp('gxGrid').hide();
    if (startTime != "" && endTime != "") {
        Ext.getCmp("cs_time").setMaxValue(endTime);
        Ext.getCmp("ce_time").setMinValue(startTime);
    } else {
        Ext.getCmp("cs_time").setMaxValue('9999/12/31');
        Ext.getCmp("ce_time").setMinValue('0001/01/01');
    }
    var row = Ext.getCmp("tierGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        ///edit by wwei0216w 2015/10/7
        Ext.getCmp('btnSave').show();  /*交換位置防止panel中顯示 出現上一次數據*/
        Tier_Load(row[0]);
        addPc.show();
    }
}


function onRemoveClick() {
    var row = Ext.getCmp("tierGrid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.schedule_id + '|';
                }
                Ext.Ajax.request({
                    url: '/ProductTier/DeleteTires',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            tierStore.load();
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                        }
                        else
                            Ext.Msg.alert(INFORMATION, DELETEFAILURE);
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

var smRe = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("relevantGrid").down('#removere').setDisabled(selections.length == 0);
        }
    }
});

var relevantGrid = Ext.create('Ext.grid.Panel', {
    plugins: [{ ptype: 'cellediting' }],
    height: 500,
    id: 'relevantGrid',
    store: relevantStore,
    selModel: smRe,
    columns: [
        { xtype: 'rownumberer', width: 25 },
        {
            text: NAME, dataIndex: 'item_name', width: 150, menuDisabled: true, sortable: false,//名稱
            editor: {
                xtype: 'textfield'
            }
        },
        {
            text: PARALLELISM_INFO, dataIndex: 'tabType', width: 120, menuDisabled: true, sortable: false,//對應表
            editor: {
                xtype: 'combobox',
                queryMode: 'local',
                editable: false,
                store: typeStore,
                displayField: 'parameterName',
                valueField: 'ParameterCode'
            },
            renderer: function (val) {
                var record = typeStore.findRecord('ParameterCode', val);
                if (record) {
                    return record.data.parameterName;
                }
                return val ? val : '';
            }
        },
        {
            text: 'Key' + VALUE, dataIndex: 'keyStr', width: 100, menuDisabled: true, sortable: false,
            editor: {
                xtype: 'combobox',
                queryMode: 'local',
                editable: false,
                store: keyStore,
                displayField: 'parameterName',
                valueField: 'ParameterCode'
            },
            renderer: function (val) {
                var record = keyStore.findRecord('ParameterCode', val);
                if (record) {
                    return record.data.parameterName;
                }
                return val ? val : '';
            }
        },
        {
            text: 'Value' + VALUE, dataIndex: 'valueStr', flex: 1, menuDisabled: true, sortable: false,
            editor: {
                xtype: 'combobox',
                queryMode: 'local',
                editable: true,
                store: valueStore,
                valueField: 'ParameterCode',
                displayField: 'parameterName'
            },
            renderer: function (val) {
                var record = valueStore.findRecord('ParameterCode', val);
                if (record) {
                    return record.data.parameterName;
                }
                return val ? val : '';
            }
        }],
    tbar: [{
        text: INSERT, handler: function () {//新增
            itemId--;///edit by ww2015/10/09
            relevantGrid.getStore().add({ id: itemId });
            //relevantStore.insert(0, { id: '0'});
        }
    }, {
        text: DELETE, id: 'removere', disabled: true, handler: function () {//删除
            var rowRe = Ext.getCmp("relevantGrid").getSelectionModel().getSelection();
            Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_VENDOR_SYSTEM_MESSAGE, rowRe.length), function (btn) {//刪除供應商系統將一同取消供應商旗下商品的排程，確定刪除選中的 {0} 條數據？
                if (btn == 'yes') {
                    var rowTmepId = '', rowIDs = '', schedule_ids = '', item_type, item_value;
                    for (var i = 0; i < rowRe.length; i++) {
                        var temp = rowRe[i].data.id;
                        if (temp < 0) {
                            relevantGrid.getStore().remove(rowRe[i]);
                        } else {
                            rowIDs += temp + ',';
                        }
                    }

                    if (rowIDs == '') {
                        return;
                    }
                    schedule_id = rowRe[0].data.schedule_Id;
                    item_type = rowRe[0].data.tabType;
                    item_value = rowRe[0].data.value1;
                    Ext.Ajax.request({
                        url: '/ProductTier/DeleteItem',
                        method: 'post',
                        params: { rowID: rowIDs, schedule_id: schedule_id, item_type: item_type, item_value: item_value },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result) {
                                relevantStore.load({
                                    params: { schedule_id: rowRe[0].data.schedule_Id }
                                });
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                            }
                            else
                                Ext.Msg.alert(INFORMATION, FAILURE);
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            })
        }
    }],
    buttons: [{
        text: SAVE,//保存
        id: 'btnSaveRe',
        listeners: {
            click: function () {
                var myMask = new Ext.LoadMask(Ext.getBody(), {
                    msg: 'Loading...'
                });
                myMask.show();
                var newList = [];
                var oldList = [];
                var updateList = [];
                var upDataStore = relevantStore.getUpdatedRecords(); //獲得修改過的store

                for (var i = 0; i < relevantStore.data.length; i++) { //查找新增數據
                    var item = relevantStore.data.items[i];
                    if (item.data.item_name == "" || item.data.tabType == "" || item.data.keyStr == "" || item.data.valueStr == "") {
                        Ext.Msg.alert(INFO, MESSAGEPROMPT);
                        myMask.hide();
                        return;
                    }
                    if (item.data.id < 0) {
                        item.data.id = 0;
                    }
                    if (item.data.id == 0) {
                        newList.push(item.data.item_name);
                        item.data.type = item.data.tabType;
                        item.data.key1 = item.data.keyStr;
                        item.data.value1 = item.data.valueStr;
                        //upDataStore[upDataStore.length] = item;
                    } else {
                        oldList.push(item.data.item_name);
                    }
                }

                ///查找重複數據
                for (var i = 0; i < newList.length; i++) {
                    for (var j = 0; j < oldList.length; j++) {
                        if (oldList[j] == newList[i]) {
                            myMask.hide();
                            Ext.Msg.alert(INFORMATION, PLEASEVERIFYDATAOFREPEAT);//沒有數據被修改
                            return;
                        }
                    }
                }


                for (var i = 0; i < upDataStore.length; i++) { //更新數據
                    var item = upDataStore[i];
                    if (!isNaN(item.data.tabType)) {
                        item.data.type = item.data.tabType;
                    }
                    if (!isNaN(item.data.keyStr)) {
                        item.data.key1 = item.data.keyStr;
                    }
                    if (!isNaN(item.data.valueStr)) {
                        item.data.value1 = item.data.valueStr;
                    }
                    if (item.data.item_name == "" || item.data.tabType == "" || item.data.keyStr == "" || item.data.valueStr == "") {
                        Ext.Msg.alert(INFO, MESSAGEPROMPT);
                        myMask.hide();
                        return;
                    }

                    for (var i = 0; i < relevantStore.data.length; i++) {
                        var itemOld = relevantStore.data.items[i];
                        if (item.data.id != itemOld.data.id) {
                            if (item.data.item_name == itemOld.data.item_name) {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, PLEASEVERIFYDATAOFREPEAT);//沒有數據被修改
                                return;
                            }
                        }
                    }
                }

                if (!upDataStore.length) {
                    myMask.hide();
                    Ext.Msg.alert(INFORMATION, NON_DATA_HAVE_EDIT);//沒有數據被修改
                    return;
                }
                var relevants = [];


                for (var i = 0; i < upDataStore.length ; i++) {
                    var record = upDataStore[i].data
                    if (record.id < 0) {
                        record.id = 0;
                    }
                    relevants.push({
                        'id': record.id,
                        'schedule_Id': schedule_id,
                        'item_name': record.item_name,
                        'type': record.type,
                        'key1': record.key1,
                        'value1': record.value1
                    });
                }
                var relevants = JSON.stringify(relevants);
                if (relevants == "[]") {
                    myMask.hide();
                    return;
                }
                Ext.Ajax.request({
                    url: '/ProductTier/SaveItem',
                    params: {
                        relevants: relevants
                    },
                    timeout: 360000,
                    success: function (response) {
                        var res = Ext.decode(response.responseText);
                        if (res) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            relevantStore.load({///add by wwei0216w 添加store加載,防止新增對應關係-->直接刪除 后導致的全部數據都被刪除
                                params: {
                                    schedule_id: schedule_id
                                }
                            })
                        } else {
                            Ext.Msg.alert(INFORMATION, DELETE_UNUSUAL);//刪除異常
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
})

var reWindow = Ext.create('Ext.window.Window', {
    title: PARALLELISM_INFORMATION_CONFIG,//對應信息設定
    id: 'reWindow',
    items: [relevantGrid],
    width: 680,
    height: 535,
    layout: 'anchor',
    closeAction: 'hide',
    modal: true,
    resizable: true,
    autoScroll: true
});

function onRelevantClick() {
    var row = Ext.getCmp("tierGrid").getSelectionModel().getSelection();
    if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        schedule_id = row[0].data.schedule_id;
        reWindow.show();
        relevantStore.load({
            params: {
                schedule_id: row[0].data.schedule_id
            }
        })
    }
}