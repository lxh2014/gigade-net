
/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/9/21
 * 電子報類型
 */
var pageSize = 25;

//列表頁的model
Ext.define('gridlistEGN', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "int" },//群組編號
        { name: "group_name", type: "string" },//群組名稱
        { name: "is_member_edm", type: "int" },//會員電子報
        { name: "is_member_edm_string", type: "string" },//會員電子報顯示
        { name: "enabled", type: "int" },//是否啟用
        { name: "sort_order", type: "int" },//群組排序。當is_member_edm為True時，該群組會顯示在會員中心的電子報訂閱畫面，此時採用這個值來決定顯示的排序。
        { name: "description", type: "string" },//群組描述文字
        { name: "group_name_list", type: "string" },
        {name:"trial_url",type:"string"},//試閱
    ],
});

//store 列表頁的數據源 
var EdmGroupNewStore = Ext.create('Ext.data.Store', {//EdmGroupNewStore
    pageSize: pageSize,
    autoDestroy: true,
    autoLoad: true,
    model: 'gridlistEGN',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetEdmGroupNewList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
//列表頁加載時候得到的數據
EdmGroupNewStore.on('beforeload', function () {
    Ext.apply(EdmGroupNewStore.proxy.extraParams,
        {
           // group_name_list: Ext.getCmp('group_name_list').getValue(),
        });
});

//每行數據前段的矩形選擇框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("EdmGroupNewGrid").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

//列表頁加載
Ext.onReady(function () {
    var EdmGroupNewGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmGroupNewGrid',
        store: EdmGroupNewStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
            new Ext.grid.RowNumberer(),//自動顯示行號
            { header: "編號", dataIndex: "group_id", align: 'center' },
            { header: "群組名稱", dataIndex: "group_name", width: 300, align: 'center' },
            { header: "會員電子報", dataIndex: "is_member_edm_string", width: 200, align: 'center' },
            {
                header: "試閱", dataIndex: "trial_url", align: 'center', width: 150,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    //return "<a href='#'>" + value + "</a>";
                    return Ext.String.format('<a href="{0}" target="_blank">{0}</a>', value);
                }
            },
            {
                header: "是否啟用", dataIndex: 'enabled', align: 'center', hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='1' id='img" + record.data.group_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='0' id='img" + record.data.group_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           {
               xtype: 'button',
               text: "新增",
               id: 'add',
               iconCls: 'ui-icon ui-icon-user-add',
               handler: onAddClick
           },
           {
               xtype: 'button',
               text: "編輯",
               id: 'edit',
               iconCls: 'ui-icon ui-icon-user-edit',
               handler: onedit
           },
           '->',
            //{
            //    xtype: 'textfield',
            //    fieldLabel: '群組名稱',
            //    labelWidth: 70,
            //    width: 180,
            //    id: 'group_name_list',
            //    name: 'group_name_list',
            //    allowBlank: false,
            //    submitValue: true,
            //    hideen:true,
            //    emptyText:'群組名稱',
            //    listeners: {
            //        specialkey: function (field, e) {
            //            if (e.getKey() == e.ENTER) {
            //                Query();
            //            }
            //        }
            //    }
            //},
            //{
            //    text: '查詢',
            //    margin: '0 10 0 10',
            //    iconCls: 'icon-search',
            //    hidden:true,
            //    handler: function () {
            //        Query(); 
            //    },
            //    listeners: {
            //        onClick: function () {
            //            if (Ext.getCmp('group_name_list') == '') {
            //                Ext.Msg.alert('提示信息', '請輸入查詢條件')
            //            }
            //        }
            //    }
            //},
            //{
            //    text: '重置',
            //    iconCls: 'ui-icon ui-icon-reset',
            //    handler: function () {
            //        Ext.getCmp('group_name_list').setValue('');//重置為空
            //    }
            //},
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmGroupNewStore,
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
            }
        },
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [EdmGroupNewGrid],// 包含两个控件 
        autoScroll: true,
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                EdmGroupNewGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });

})
/*************************************************************************************查询信息*************************************************************************************************/

function Query(x) {
    if (Ext.getCmp('group_name_list').getValue() == '') {
        Ext.Msg.alert('提示信息', '請輸入查詢條件');
    }
    else {
        Ext.getCmp('EdmGroupNewGrid').store.loadPage(1, {
            params: {
                group_name: Ext.getCmp('group_name_list').getValue(),
            }
        });
    }

}

/*********************啟用/禁用**********************/
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/EdmNew/UpdateStats",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "post",
        type: 'text',
        success: function (msg) {
            EdmGroupNewStore.load();
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

/*******************添加信息*****************************************/

function onAddClick() {
    editFunction(null, EdmGroupNewStore);
}
/*********************************編輯*****************************************/

onedit = function () {
    var row = Ext.getCmp("EdmGroupNewGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
         Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
         Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        //Ext.Msg.alert(row[0].data.name);
        editFunction(row[0], EdmGroupNewStore);
    }
}