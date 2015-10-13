/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/10/6
 * 排程
 */
var currentRecord = { data: {} };
var pageSize = 25;


//列表頁的model (Master)
Ext.define('gridlistMaster', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "int" },
        { name: "schedule_code", type: "string" },
        { name: "schedule_name", type: "string" },
        { name: "schedule_api", type: "string" },
        { name: "schedule_description", type: "string" },
        { name: "schedule_state", type: "int" },
        //{ name: "sschedule_state", type: "string" },
        { name: "previous_execute_time", type: "int" },
        { name: "show_previous_execute_time", type: "string" },
        { name: "next_execute_time", type: "int" },
        { name: "show_next_execute_time", type: "string" },
        { name: "schedule_period_id", type: "int" },
        { name: "create_username", type: "string" },
        { name: "create_time", type: "int" },
        { name: "show_create_time", type: "string" },
        { name: "change_username", type: "string" },
        { name: "change_time", type: "int" },
        { name: "show_change_time", type: "string" }
    ],
});

//master 列表頁的數據源 
var ScheduleStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
   // autoLoad:true,
    autoDestroy: true,
    model: 'gridlistMaster',
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetScheduleMasterList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


ScheduleStore.on("beforeload", function () {
    Ext.apply(ScheduleStore.proxy.extraParams, {

    })
})

// config 的model
Ext.define('GIGADE.Config', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowid', type: 'string' },
        { name: 'schedule_code', type: 'string' },
        { name: 'parameterCode', type: 'string' },
        { name: 'value', type: 'string' },
        { name: 'parameterName', type: 'string' },
        { name: "create_username", type: "string" },
        { name: "create_time", type: "int" },
        { name: "show_create_time", type: "string" },
        { name: "show_change_time", type: "string" },
        { name: "change_username", type: "string" },
        { name: "change_time", type: "int" },
    ]
});
var Schedule_Config_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Config',
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetScheduleConfigList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//
Ext.define('GIGADE.ScheduleCode', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "schedule_code", type: "string" },
    ],
});

var Schedule_Code_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.ScheduleCode',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetScheduleMasterList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Schedule_Config_Store.on("beforeload", function () {
    Ext.apply(Schedule_Config_Store.proxy.extraParams, {
        schedule_code: Ext.getCmp("schedule_code").getValue(),
    })
})

// period 的model
Ext.define('GIGADE.Period', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowid', type: 'string' },
        { name: 'schedule_code', type: 'string' },
        { name: "create_username", type: "string" },
        { name: "create_time", type: "int" },
        { name: "change_username", type: "string" },
        { name: "change_time", type: "int" },
         { name: 'period_type', type: 'int' },
         {name:"show_period_type",type:'string'},
        { name: 'period_nums', type: 'int' },
        { name: 'show_begin_datetime', type: 'date' },
        { name: 'current_nums', type: 'int' },
        { name: 'limit_nums', type: 'int' },
        { name: "show_create_time", type: "string" },
        { name: "show_change_time", type: "string" }
    ]
});


var Schedule_Period_Store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.Period',
    proxy: {
        type: 'ajax',
        url: '/ScheduleService/GetSchedulePeriodList',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

Schedule_Period_Store.on("beforeload", function () {
    Ext.apply(Schedule_Period_Store.proxy.extraParams, {
        schedule_code: Ext.getCmp("schedule_code").getValue(),
    })
})
//每行數據前段的矩形選擇框
var sm_master = Ext.create('Ext.selection.CheckboxModel', {// master 矩形選擇框
    listeners: {
        selectionchange: function (sm_master, selections) {
            Ext.getCmp("masterGiftList").down('#edit_master').setDisabled(selections.length == 0);
            Ext.getCmp("masterGiftList").down('#delete_master').setDisabled(selections.length == 0);
            Ext.getCmp("masterGiftList").down('#runonce_master').setDisabled(selections.length == 0);
        }
    }
});
var sm_config = Ext.create('Ext.selection.CheckboxModel', {// config 矩形選擇框
    listeners: {
        selectionchange: function (sm_config, selections) {
            Ext.getCmp("detailist1").down('#edit_config').setDisabled(selections.length == 0);
            Ext.getCmp("detailist1").down('#delete_config').setDisabled(selections.length == 0);
        }
    }
});
var sm_period = Ext.create('Ext.selection.CheckboxModel', {// period 矩形選擇框
    listeners: {
        selectionchange: function (sm_period, selections) {
            Ext.getCmp("detailist2").down('#edit_period').setDisabled(selections.length == 0);
            Ext.getCmp("detailist2").down('#delete_period').setDisabled(selections.length == 0);
        }
    }
});

//左邊Master 列表頁
var masterGiftList = Ext.create('Ext.grid.Panel', {
    id: 'masterGiftList',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    columnLines: true,
    store: ScheduleStore,
    columns: [                      //顯示master
        { header: '編號', dataIndex: 'rowid', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
         {
             header: "排程狀態", dataIndex: 'schedule_state', align: 'center', width: 60, hidden: false,
             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                 if (value) {
                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowid + ")'><img hidValue='1' id='img" + record.data.rowid + "' src='../../../Content/img/icons/accept.gif'/></a>";
                 } else {
                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.rowid + ")'><img hidValue='0' id='img" + record.data.rowid + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                 }
             }
         },
        { header: '排程Code', dataIndex: 'schedule_code', align: 'left', width: 80, menuDisabled: true, sortable: false, align: 'center' },
        { header: '排程名稱', dataIndex: 'schedule_name', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: 'contriller/action', dataIndex: 'schedule_api', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '排程描述', dataIndex: 'schedule_description', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: 'schedule_period表主鍵', dataIndex: 'schedule_period_id', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
       { header: '創建人', dataIndex: 'create_username', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
       { header: '修改人', dataIndex: 'change_username', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
        { header: '上次執行時間', dataIndex: 'show_previous_execute_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '下次執行時間', dataIndex: 'show_next_execute_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '創建時間', dataIndex: 'show_create_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '修改時間', dataIndex: 'show_change_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
    ],
    tbar: [
     { xtype: 'button', text: "添加", id: 'add_master', iconCls: 'icon-user-add', handler: add_master },//添加按鈕
     { xtype: 'button', text: "編輯", id: 'edit_master', iconCls: 'icon-user-edit', disabled: true, handler: onedit_master },//編輯按鈕  包括 添加 刪除 修改 功能
     { xtype: 'button', text: "刪除", id: 'delete_master', iconCls: 'icon-user-remove', disabled: true, handler: ondelete_master },
     { xtype: 'button', text: "立即執行", id: 'runonce_master', iconCls: 'icon-user-edit', disabled: true, handler: onrunonce_master },
     '->',
     { xtype: 'button', text: "查詢", id: 'grid_btn_search', iconCls: 'ui-icon ui-icon-search', width: 65, handler: Search },
    ],
    bbar: Ext.create('Ext.PagingToolbar', {
        store: ScheduleStore,
        pageSize: pageSize,
        displayInfo: true,//是否顯示數據信息
        displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
        emptyMsg: NOTHING_DISPLAY
    }),
    listeners: {
        scrollershow: function (scroller) {
            if (scroller && scroller.scrollEl) {
                scroller.clearManagedListeners();
                scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
            }
        },
        itemclick: function (view, record, item, index, e) {
            LoadDetail(currentRecord = record);
        },
        resize: function () {
            this.doLayout();
        }
    },
    selModel: sm_master,
})

// 中間的panel
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
            title: '排程服務',
            autoScroll: true,
            frame: false,
            items: [
                {
                    xtype: 'container',
                    autoScroll: true,
                    defaults: { margin: '0 5 5 10', labelWidth: 60, autoScroll: true, width: 1210 },
                    items: [
                          {
                              id: 'schedule_code',
                              xtype: 'textfield',
                              fieldLabel: "schedule_code",
                              maxLength: 18,
                              labelWidth: 70,
                              //regex: /^\d+$/,
                              //regexText: '请输入正確的編號,訂單編號,行動電話進行查詢',
                              name: 'schedule_code',
                              allowBlank: true,
                              hidden: true,
                          },
                          {
                              flex: 3.0,
                              title: '排程參數信息',
                              xtype: 'gridpanel',
                              id: 'detailist1',
                              autoScroll: true,
                              columnLines: true,
                              height: 300,
                              frame: false,
                              Height: 500,
                              store: Schedule_Config_Store,
                              columns: [
                                 // { header: '序號', xtype: 'rownumberer', width: 46, align: 'center' },
                                  { header: '編號', dataIndex: 'rowid', align: 'left', width: 40, menuDisabled: true, sortable: false, align: 'center' },
                                  { header: '排程Code', dataIndex: 'schedule_code',width:100, align: 'center',  menuDisabled: true, sortable: false },
                                   { header: '參數碼', dataIndex: 'parameterCode', width: 100, align: 'center',  menuDisabled: true, sortable: false },
                                   { header: '參數名稱', dataIndex: 'parameterName',width:100, align: 'center',  menuDisabled: true, sortable: false },
                                  { header: '參數值', dataIndex: 'value', align: 'center', width: 100,  menuDisabled: true, sortable: false },
                                  { header: '創建人', dataIndex: 'create_username', width: 100, align: 'center',menuDisabled: true, sortable: false },
                                  { header: '修改人', dataIndex: 'change_username', width: 100, align: 'center', menuDisabled: true, sortable: false },
                                   { header: '創建時間', dataIndex: 'show_create_time', width: 150, align: 'center', menuDisabled: true, sortable: false },
                                  { header: '修改時間', dataIndex: 'show_change_time', width:150, align: 'center',  menuDisabled: true, sortable: false },
                              ],
                              tbar: [
            { xtype: 'button', text: "添加", id: 'add_config', iconCls: 'icon-user-add', handler: add_config },//添加按鈕
            { xtype: 'button', text: "編輯", id: 'edit_config', disabled: true, iconCls: 'icon-user-edit', handler: onedit_config},//編輯按鈕  
            { xtype: 'button', text: "刪除", id: 'delete_config', disabled: true, iconCls: 'icon-user-remove', handler: ondelete_config },

                              ],
                              selModel: sm_config
                          },

                        {
                            flex: 3.0,
                            title: '排程執行信息',
                            xtype: 'gridpanel',
                            id: 'detailist2',
                            autoScroll: true,
                            columnLines: true,
                            height: document.documentElement.clientHeight - 400,
                            frame: false,
                            store: Schedule_Period_Store,
                            columns: [
                                //{ header: '序號', xtype: 'rownumberer', width: 46, align: 'center' },
                                 { header: '編號', dataIndex: 'rowid', align: 'left', width: 40, menuDisabled: true, sortable: false, align: 'center' },
                                { header: '排程Code', dataIndex: 'schedule_code', width: 100, align: 'center',  menuDisabled: true, sortable: false },
                                 { header: '執行頻率方式', dataIndex: 'show_period_type', align: 'center', width: 100, menuDisabled: true, sortable: false },
                                { header: '執行頻率倍數', dataIndex: 'period_nums', align: 'center', width: 100, menuDisabled: true, sortable: false },
                                { header: '當前已執行次數', dataIndex: 'current_nums', align: 'center', width: 80,  menuDisabled: true, sortable: false },
                               { header: '次數限制', dataIndex: 'limit_nums', align: 'center', width: 80, menuDisabled: true, sortable: false },
                               { header: '創建人', dataIndex: 'create_username', align: 'center', width: 80,  menuDisabled: true, sortable: false },
                               { header: '修改人', dataIndex: 'change_username', align: 'center', width: 80, menuDisabled: true, sortable: false },
                                { header: '啟用時間', dataIndex: 'show_begin_datetime', align: 'center', width: 150, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s') },
                               { header: '創建時間', dataIndex: 'show_create_time', align: 'center', width: 150, menuDisabled: true, sortable: false },
                               { header: '修改時間', dataIndex: 'show_change_time', align: 'center', width: 150,  menuDisabled: true, sortable: false },
                            ],
                            tbar: [
          { xtype: 'button', text: "添加", id: 'add_period', iconCls: 'icon-user-add', handler: add_period },//添加按鈕
          { xtype: 'button', text: "編輯", id: 'edit_period', disabled: true, iconCls: 'icon-user-edit', handler: onedit_period },//編輯按鈕  
          { xtype: 'button', text: "刪除", id: 'delete_period', disabled: true, iconCls: 'icon-user-remove', handler: ondelete_period },
     
                            ],
                            selModel: sm_period
                        },

                    ]
                }]
        }],
})

Ext.onReady(function () {
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
            items: masterGiftList
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

function LoadDetail(record) {
    if (record.data.rowid == undefined || record.data.rowid == 0) {
        Ext.getCmp('center').getForm().reset();
        Schedule_Config_Store.removeAll();
        Schedule_Period_Store.removeAll();
    }
else 
  {
        Ext.getCmp("schedule_code").setValue(record.data.schedule_code);
        //center.getForm().loadRecord(record);
        Schedule_Config_Store.load();
        Schedule_Period_Store.load();
    }
}
function Search() {
    Ext.getCmp('masterGiftList').store.loadPage(1, {
        params: {

        }
    });
}

/*************************************************************************************啟用/禁用*************************************************************************************************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/ScheduleService/UpdateStats_Schedule_master",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            ScheduleStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}


/*************************************************************************************編輯_master*************************************************************************************************/

 function onedit_master () {
    var row = Ext.getCmp("masterGiftList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("未選中任何行!");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("只能选择一行!");
    } else if (row.length == 1) {
        editFunction_master(row[0], ScheduleStore);
    }
 }

 /*************************************************************************************編輯_config*************************************************************************************************/

 function onedit_config() {
     var row = Ext.getCmp("detailist1").getSelectionModel().getSelection();
     if (row.length == 0) {
         Ext.Msg.alert("未選中任何行!");
     }
     else if (row.length > 1) {
         Ext.Msg.alert("只能选择一行!");
     } else if (row.length == 1) {
         editFunction_config(row[0], Schedule_Config_Store);
     }
 }

 /*************************************************************************************編輯_period*************************************************************************************************/
 function onedit_period() {
     var row = Ext.getCmp("detailist2").getSelectionModel().getSelection();
     if (row.length == 0) {
         Ext.Msg.alert("未選中任何行!");
     }
     else if (row.length > 1) {
         Ext.Msg.alert("只能选择一行!");
     } else if (row.length == 1) {
         editFunction_period(row[0], Schedule_Period_Store);
     }
 }

/*************************************************************************************添加信息_master*************************************************************************************************/

function add_master() {
    editFunction_master(null, ScheduleStore);
}
/*************************************************************************************添加信息_config*************************************************************************************************/
function add_config() {
    editFunction_config(null, Schedule_Config_Store);
}
/*************************************************************************************添加信息_period*************************************************************************************************/

function add_period() {
    editFunction_period(null, Schedule_Period_Store);
}
/*************************************************************************************刪除_master*************************************************************************************************/

function ondelete_master() {
    var row = Ext.getCmp("masterGiftList").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("未選中任何行!");
    }

    else {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {

                    rowIDs += row[i].data.rowid + ',';//可以刪除多條數據記錄

                    //  rowIDs += row[i].data.id//刪除一條數據記錄

                    Ext.Msg.alert(rowIDs);
                }
                Ext.Ajax.request({
                    //控制器下的delete方法
                    url: '/ScheduleService/ScheduleMasterDelete',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            // ScheduleStore.loadPage(1);
                            ScheduleStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "無法刪除!");
                            //ScheduleStore.loadPage(1);
                            ScheduleStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("刪除失敗!");
                    }
                });
            }
        });
    }
}

/*************************************************************************************刪除_config*************************************************************************************************/

function ondelete_config() {
    var row = Ext.getCmp("detailist1").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("未選中任何行!");
    }

    else {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {

                    rowIDs += row[i].data.rowid + ',';//可以刪除多條數據記錄

                    //  rowIDs += row[i].data.id//刪除一條數據記錄

                    Ext.Msg.alert(rowIDs);
                }
                Ext.Ajax.request({
                    //控制器下的delete方法
                    url: '/ScheduleService/ScheduleConfigDelete',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            // ScheduleStore.loadPage(1);
                            Schedule_Config_Store.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "無法刪除!");
                            //ScheduleStore.loadPage(1);
                            Schedule_Config_Store.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("刪除失敗!");
                    }
                });
            }
        });
    }
}

/*************************************************************************************刪除_period*************************************************************************************************/

function ondelete_period() {
    var row = Ext.getCmp("detailist2").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("未選中任何行!");
    }

    else {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("刪除選中" + row.length + "條數據?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {

                    rowIDs += row[i].data.rowid + ',';//可以刪除多條數據記錄

                    //  rowIDs += row[i].data.id//刪除一條數據記錄

                    Ext.Msg.alert(rowIDs);
                }
                Ext.Ajax.request({
                    //控制器下的delete方法
                    url: '/ScheduleService/SchedulePeriodDelete',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "刪除成功!");
                            // ScheduleStore.loadPage(1);
                            Schedule_Period_Store.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "無法刪除!");
                            //ScheduleStore.loadPage(1);
                            Schedule_Period_Store.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("刪除失敗!");
                    }
                });
            }
        });
    }
}

/*************************************************************************************立即執行選中的排程_master*************************************************************************************************/
function onrunonce_master() {
    var row = Ext.getCmp("masterGiftList").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("未選中任何行!");
    }

    else {
        //  var id = Ext.getCmp('id').getValue();
        Ext.Msg.confirm('提示', Ext.String.format("立即執行選中的" + row.length + "條排程?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {

                    rowIDs += row[i].data.schedule_api + '&' + row[i].data.schedule_code + ',';//可以執行多條數據記錄                  
                }
                Ext.Ajax.request({
                    //控制器下的delete方法
                    url: '/ScheduleService/ScheduleMasterRunOnce',
                    method: 'post',
                    params: { id: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "執行成功!");
                            // ScheduleStore.loadPage(1);
                            ScheduleStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "執行失敗!");
                            //ScheduleStore.loadPage(1);
                            ScheduleStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("執行失敗!");
                    }
                });
            }
        });
    }
}
