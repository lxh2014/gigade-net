/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/9/30
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

//store 列表頁的數據源 
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
        { name: 'description', type: 'string' },
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
        { name: 'period_nums', type: 'int' },
         { name: 'show_begin_datetime', type: 'string' },
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

//每行數據前段的矩形選擇框
var sm_master = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm_master, selections) {
            Ext.getCmp("masterGiftList").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
var sm_config = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm_config, selections) {
            Ext.getCmp("detailist1").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
var sm_period = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm_period, selections) {
            Ext.getCmp("detailist2").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Schedule_Period_Store.on("beforeload", function () {
    Ext.apply(Schedule_Period_Store.proxy.extraParams, {
        schedule_code: Ext.getCmp("schedule_code").getValue(),
    })
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
            title: 'Config  Period',
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
                              title: 'config',
                              xtype: 'gridpanel',
                              id: 'detailist1',
                              autoScroll: true,
                              columnLines: true,
                              height: 300,
                              frame: false,
                              Height: 500,
                              store: Schedule_Config_Store,
                              columns: [
                                  { header: '序號', xtype: 'rownumberer', width: 46, align: 'center' },
                                  { header: '排程Code', dataIndex: 'schedule_code', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                   { header: '參數碼', dataIndex: 'parameterCode', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                  { header: '參數值', dataIndex: 'value', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                  { header: '參數作用', dataIndex: 'description', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                  { header: '創建人', dataIndex: 'create_username', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                 { header: '創建時間', dataIndex: 'show_create_time', width:150, align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                  { header: '修改人', dataIndex: 'change_username', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                  { header: '修改時間', dataIndex: 'show_change_time', width:150, align: 'center', flex: 1, menuDisabled: true, sortable: false },
                              ],
                              tbar: [
                                  {
                                      text: '新增', id: 'add_config', iconCls: 'icon-user-add',handler: function () {
                                          detailAdd(null, Schedule_Config_Store);
                                      }
                                  },
                                  { text: '修改', id: 'edit_config', handler: detailEdit, iconCls: 'icon-user-edit', disabled: true },
                                  {
                                      text: '刪除', id: 'delete_config', iconCls: 'icon-user-remove', disabled: true, handler: function () {
                                          detailDelete(Schedule_Config_Store);
                                      }
                                  }],
                              selModel: sm_config
                          },

                        {
                            flex: 3.0,
                            title: 'period',
                            xtype: 'gridpanel',
                            id: 'detailist2',
                            autoScroll: true,
                            columnLines: true,
                            height: document.documentElement.clientHeight - 400,
                            frame: false,
                            store: Schedule_Period_Store,
                            columns: [
                                { header: '序號', xtype: 'rownumberer', width: 46, align: 'center' },
                                { header: '排程Code', dataIndex: 'schedule_code', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                                 { header: '執行頻率方式', dataIndex: 'period_type', align: 'center', width: 80,  menuDisabled: true, sortable: false },
                                { header: '執行頻率倍數', dataIndex: 'period_nums', align: 'center', width: 80, menuDisabled: true, sortable: false },
                                { header: '啟用時間', dataIndex: 'show_begin_datetime', align: 'center', width: 150, flex: 1, menuDisabled: true, sortable: false },
                                { header: '當前已執行次數', dataIndex: 'current_nums', align: 'center', width: 50, flex: 1, menuDisabled: true, sortable: false },
                               { header: '次數限制', dataIndex: 'limit_nums', align: 'center', width: 50, flex: 1, menuDisabled: true, sortable: false },
                               { header: '創建人', dataIndex: 'create_username', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                              { header: '創建時間', dataIndex: 'show_create_time', align: 'center',width:150, flex: 1, menuDisabled: true, sortable: false },
                               { header: '修改人', dataIndex: 'change_username', align: 'center', flex: 1, menuDisabled: true, sortable: false },
                               { header: '修改時間', dataIndex: 'show_change_time', align: 'center', width: 150, flex: 1, menuDisabled: true, sortable: false },
                            ],
                            tbar: [
                                {
                                    text: '新增', id: 'add_period', iconCls: 'icon-user-add', handler: function () {
                                        detailAdd(null, Schedule_Period_Store);
                                    }
                                },
                                { text: '修改', id: 'edit_period', disabled: true, iconCls: 'icon-user-edit', handler: detailEdit, disabled: true },
                                {
                                    text: '刪除', id: 'delete_period', disabled: true, iconCls: 'icon-user-remove', handler: function () {
                                        detailDelete(Schedule_Period_Store);
                                    }
                                }],
                            selModel: sm_period
                        },

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
        iconCls: 'ui-icon ui-icon-reset',
        handler: function () {
            this.up('form').getForm().reset();
        }
    }, {
        text: '取消',
        id: 'btn_cancel',
        iconCls: 'ui-icon ui-icon-cancel',
        handler: function () {
            Ext.getCmp('west-region-container').setDisabled(false);
        }
    }]
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

//左邊Master 列表頁
var masterGiftList = Ext.create('Ext.grid.Panel', {
    id: 'masterGiftList',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    store: ScheduleStore,
    //dockedItems: [{
    //    id: 'dockedItem',
    //    xtype: 'toolbar',
    //    layout: 'column',
    //    dock: 'top',
    //    items: [
    //        {
    //            xtype: 'button',
    //            text: '查詢',
    //            id: 'grid_btn_search',
    //            iconCls: 'ui-icon ui-icon-search',
    //            margin: ' 0 0 5 10',
    //            width: 65,
    //            handler: Search
    //        }, ]
    //}],
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
        { header: '排程Code', dataIndex: 'schedule_code', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
        { header: '排程名稱', dataIndex: 'schedule_name', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: 'contriller/action', dataIndex: 'schedule_api', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '排程描述', dataIndex: 'schedule_description', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '上次執行時間', dataIndex: 'show_previous_execute_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '下次執行時間', dataIndex: 'show_next_execute_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: 'schedule_period表主鍵', dataIndex: 'schedule_period_id', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
       { header: '創建人', dataIndex: 'create_username', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
       { header: '創建時間', dataIndex: 'show_create_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
       { header: '修改人', dataIndex: 'change_username', align: 'left', width: 60, menuDisabled: true, sortable: false, align: 'center' },
       { header: '修改時間', dataIndex: 'show_change_time', align: 'left', width: 150, menuDisabled: true, sortable: false, align: 'center' },
    ],
    tbar: [
     { xtype: 'button', text: "添加", id: 'add_master', iconCls: 'icon-user-add', handler: add },//添加按鈕
     { xtype: 'button', text: "編輯", id: 'edit_master',  iconCls: 'icon-user-edit', handler:onedit},//編輯按鈕  包括 添加 刪除 修改 功能
     //{ xtype: 'button', text: "刪除", id: 'add_master', iconCls: 'icon-user-remove', handler: ondelete },
     // { xtype: 'button', text: "新增", id: 'add_master', iconCls: 'icon-user-add', width: 65 },//添加按鈕
     //{ xtype: 'button', text: "修改", id: 'edit_master', disabled: true, iconCls: 'icon-user-edit', width: 65 },//編輯按鈕  包括 添加 刪除 修改 功能
     { xtype: 'button', text: "刪除", id: 'delete_master', iconCls: 'icon-user-remove', width: 65 },
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

////複選框列
//var sm = Ext.create('Ext.selection.CheckboxModel', {
//    listeners: {
//        selectionchange: function (sm, selections) {
//            //Ext.getCmp("edit").setDisabled(selections.length == 0);
//            // Ext.getCmp("delete").setDisabled(selections.length == 0);
//        }
//    }
//});

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




function detailAdd(row, store) {

    //  GiftTypeStore.load();

    var detailAddFrm = Ext.create('Ext.form.Panel', {
        id: 'detailAddFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        defaults: { msgTarget: "side", labelWidth: 80 },
        items: [
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
                    if (row.data.gift_type == 1) {
                        Ext.getCmp("g_product_id").show();
                    }
                    else {
                        Ext.getCmp("gift_ware").show();
                    }
                    detailAddFrm.getForm().loadRecord(row);
                }
                else {
                    this.up('form').getForm().reset();
                }
            }
        }, {
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            is_member_edm: Ext.htmlEncode(Ext.getCmp('is_member_edm').getValue().ignore_stockVal),
                            trial_url: Ext.htmlEncode(Ext.getCmp('trial_url').getValue()),
                            sort_order: Ext.htmlEncode(Ext.getCmp('sort_order').getValue()),
                            description: Ext.htmlEncode(Ext.getCmp('description').getValue()),
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, "保存成功! ");
                                store.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                            }
                        },
                        failure: function () {
                            Ext.Msg.alert(INFORMATION, "保存失敗! ");
                            editWin.close();
                        }
                    });
                }
            }
        }, ]
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

    var sms = Ext.getCmp("detailist1").getSelectionModel().getSelection();
    if (sms.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (sms.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (sms.length == 1) {
        detailAdd(sms[0], Schedule_Config_Store);
    }

}

function detailDelete(store) {
    var row = Ext.getCmp("detailist1").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
        if (btn == 'yes') {
            store.remove(row);
        }
    });
}



function Search() {

    // Ext.getCmp('grid_event_id').setValue(Ext.getCmp('grid_event_id').getValue().replace(/\s+/g, ","));
    // if (!Ext.getCmp('grid_event_id').isValid()) return;
    Ext.getCmp('masterGiftList').store.loadPage(1, {
        params: {

        }
    });
}


function Save() {

}


/*********************啟用/禁用**********************/
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


/*************************************************************************************編輯*************************************************************************************************/

 function onedit () {
    var row = Ext.getCmp("masterGiftList").getSelectionModel().getSelection();
    if (row.length == 0) {
        // Ext.Msg.alert(INFORMATION, NO_SELECTION);
        Ext.Msg.alert("未選中任何行!");
    }
    else if (row.length > 1) {
        // Ext.Msg.alert(INFORMATION, ONE_SELECTION);
        Ext.Msg.alert("只能选择一行!");
    } else if (row.length == 1) {
        //Ext.Msg.alert(row[0].data.name);
        editFunction(row[0], ScheduleStore);
    }
}


/*************************************************************************************添加信息*************************************************************************************************/

function add() {
    editFunction(null, ScheduleStore);
}

/*************************************************************************************添加 編輯 框*************************************************************************************************/
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
         url: '/ScheduleService/SaveScheduleMasterInfo',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: 'rowid',
                id: 'rowid',
                name: 'rowid',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: '排程Code',
                id: 'schedule_code',
                name: 'schedule_code',
                allowBlank:false,
            },
            {
                xtype: 'textfield',
                fieldLabel: '排程名稱',
                id: 'schedule_name',
                name: 'schedule_name',
                allowBlank: false,
            },
            {
                xtype: 'textfield',
                fieldLabel: 'contriller/action',
                id: 'schedule_api',
                name: 'schedule_api',
                allowBlank: false,
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '排程描述',
                 id: 'schedule_description',
                 name: 'schedule_description',
                 allowBlank: false,
             },
              //{
              //    xtype: 'textfield',
              //    fieldLabel: '排程狀態',
              //    id: 'schedule_state',
              //    name: 'schedule_state'
              //},
                {
                    xtype: 'textfield',
                    fieldLabel: '下次執行的記錄',
                    id: 'schedule_period_id',
                    name: 'schedule_period_id',
                    allowBlank: false,
                },
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                  {
                      fieldLabel: '排程狀態(是否啟用)',
                      xtype: 'radiogroup',
                      id: 'schedule_state',
                      labelWidth: 130,
                      width: 260,
                      defaults: {
                          name: 'ignore_stockVal'
                      },
                      columns: 2,
                      items: [
                          { id: 'id1', boxLabel: "是", inputValue: '1', checked: true },
                      //{ id: 'id1', boxLabel: "是", inputValue: '1' },
                      { id: 'id2', boxLabel: "否", inputValue: '0' }
                      ]
                  }
                  ]
              },

               //{
               //    xtype: 'textfield',
               //    fieldLabel: '創建人',
               //    id: 'create_user',
               //    name: 'create_user'
               //},
               // {
               //     xtype: 'textfield',
               //     fieldLabel: '修改人',
               //     id: 'change_user',
               //     name: 'change_user'
               // },
               
                  //{
                  //    xtype: 'fieldcontainer',
                  //    layout: 'hbox',
                  //    fieldLabel: '上次執行時間',
                  //    items: [
                  //        {
                  //            xtype: 'datefield',
                  //            id: 'previous_execute_time',
                  //            name: 'previous_execute_time',
                  //            format: 'Y-m-d',
                  //            editable: false,
                  //            allowBlank: false,
                  //        },
                  //        {
                  //            xtype: 'displayfield',
                  //            value: '<span style="color:red">※上次執行時間</span>'
                  //        }
                  //    ]
                  //},
                  //   {
                  //       xtype: 'fieldcontainer',
                  //       layout: 'hbox',
                  //       fieldLabel: '下次執行時間',
                  //       items: [
                  //           {
                  //               xtype: 'datefield',
                  //               id: 'next_execute_time',
                  //               name: 'next_execute_time',
                  //               format: 'Y-m-d',
                  //               editable: false,
                  //               allowBlank: false,
                  //           },
                  //           {
                  //               xtype: 'displayfield',
                  //               value: '<span style="color:red">※下次執行時間</span>'
                  //           }
                  //       ]
                  //   },
                  //   {
                  //       xtype: 'fieldcontainer',
                  //       layout: 'hbox',
                  //       fieldLabel: '創建日期',
                  //       items: [
                  //           {
                  //               xtype: 'datefield',
                  //               id: 'create_time',
                  //               name: 'create_time',
                  //               format: 'Y-m-d',
                  //               editable: false,
                  //               allowBlank: false,
                  //           },
                  //           {
                  //               xtype: 'displayfield',
                  //               value: '<span style="color:red">※創建日期</span>'
                  //           }
                  //       ]
                  //   },
                  //     {
                  //         xtype: 'fieldcontainer',
                  //         layout: 'hbox',
                  //         fieldLabel: '更改日期',
                  //         items: [
                  //             {
                  //                 xtype: 'datefield',
                  //                 id: 'change_time',
                  //                 name: 'change_time',
                  //                 format: 'Y-m-d',
                  //                 editable: false,
                  //                 allowBlank: false,
                  //             },
                  //             {
                  //                 xtype: 'displayfield',
                  //                 value: '<span style="color:red">※更改日期</span>'
                  //             }
                  //         ]
                  //     },
        ],

        // 点击保存按钮后  提示信息 
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '保存',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                rowid: Ext.htmlEncode(Ext.getCmp('rowid').getValue()),
                                schedule_code: Ext.htmlEncode(Ext.getCmp('schedule_code').getValue()),
                                schedule_name: Ext.htmlEncode(Ext.getCmp('schedule_name').getValue()),
                                schedule_api: Ext.htmlEncode(Ext.getCmp('schedule_api').getValue()),
                                schedule_description: Ext.htmlEncode(Ext.getCmp('schedule_description').getValue()),
                                schedule_state: Ext.htmlEncode(Ext.getCmp('schedule_state').getValue().ignore_stockVal),
                                //create_user: Ext.htmlEncode(Ext.getCmp('create_user').getValue()),
                                //change_user: Ext.htmlEncode(Ext.getCmp('change_user').getValue()),
                                schedule_period_id: Ext.htmlEncode(Ext.getCmp('schedule_period_id').getValue()),
                                //previous_execute_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('previous_execute_time').getValue()), 'Y-m-d H:i:s')),
                                //next_execute_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('next_execute_time').getValue()), 'Y-m-d H:i:s')),
                                //create_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('create_time').getValue()), 'Y-m-d H:i:s')),
                                //change_time: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('change_time').getValue()), 'Y-m-d H:i:s')),
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "保存成功! ");
                                    store.load();
                                    editWin.close();
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                    editWin.close();
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, "保存失敗! ");
                                editWin.close();
                            }
                        });
                    }
                }
            },
        ]
    });


    //点击关闭按钮后  提示信息
    //一个指定的打算作为一个应用程序窗口的面板。
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增信息",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 460,
        height: 280,
        layout: 'fit',//布局样式
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,//false 禁止調整windows窗體的大小
        // reaizable:true,// true  允許調整windows窗體的大小
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
                    if (row.data.is_member_edm_string.trim().length == 0) {
                        Ext.getCmp("id1").setValue(false);
                        Ext.getCmp("id2").setValue(true);
                    } else {
                        Ext.getCmp("id1").setValue(true);
                        Ext.getCmp("id2").setValue(false);
                    }
                    editFrm.getForm().loadRecord(row);

                }
                else {

                    editFrm.getForm().reset();

                }
            }
        }
    });


    editWin.show();

}

/*************************************************************************************添加信息*************************************************************************************************/