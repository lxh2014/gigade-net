/*
 * Copyright (c)J01 
 * 作   者：yachao1120j
 * CreateTime :2015/9/22
 * 電子報範本
 */
var pageSize = 25;

//列表頁的model
Ext.define('gridlistET', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "template_id", type: "int" },//EDM範本代碼
        { name: "template_name", type: "string" },//EDM範本名稱
        { name: "edit_url", type: "string" },//EDM編輯者，選擇該範本後，用來給編輯者提供該範本相關資料的網頁
        { name: "content_url", type: "string" },//最終用來產出EDM內容的網頁，會被程式呼叫，以便取得EDM郵件內容。產出的內容會用來寫入到mail_request的body欄位
        { name: "enabled", type: "int" },//是否啟用
        { name: "template_create_userid", type: "int" },//建立者
        { name: "template_update_userid", type: "int" },//修改者
        { name: "template_updatedate",type:"datetime" },//更新時間
    ],
});

//store 列表頁的數據源 
var EdmTemplateStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    autoDestroy: true,
    model: 'gridlistET',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetEdmTemplateList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

//每行數據前段的矩形選擇框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("EdmTemplateGrid").down('#edit').setDisabled(selections.length == 0);
        }
    }
});


//列表頁加載
Ext.onReady(function () {
    var EdmTemplateGrid = Ext.create('Ext.grid.Panel', {
        id: 'EdmTemplateGrid',
        store: EdmTemplateStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
             { header: "編號", dataIndex: "template_id", align: 'center' },
            { header: "範本名稱", dataIndex: "template_name", width: 300, align: 'center' },
            { header: "內容編輯網址", dataIndex: "edit_url", width: 200, align: 'center' },
             { header: "內容產生網址", dataIndex: "content_url", width: 200, align: 'center' },
            {
                header: "是否啟用", dataIndex: 'enabled', align: 'center', hidden: false,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='1' id='img" + record.data.group_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    } else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.group_id + ")'><img hidValue='0' id='img" + record.data.group_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            },
             { header: "建立者", dataIndex: "template_create_userid", width: 300, align: 'center' },
            { header: "修改者", dataIndex: "template_update_userid", width: 200, align: 'center' },
             { header: "更新時間", dataIndex: "template_updatedate", width: 200, align: 'center' },
        ],
        tbar: [
           //{ xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
           //{ xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', handler: onedit },
           { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add' },
           { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit' },
           '->', {
               text: '查詢',
               // margin: '0 8 0 8',
               margin: '0 10 0 10',
               iconCls: 'icon-search',
               handler: function () {
                   Query();
               }
           },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmTemplateStore,
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
        items: [EdmTemplateGrid],// 包含两个控件 
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
    Ext.getCmp('EdmTemplateGrid').store.loadPage(1, {
        params: {

        }
    });
}