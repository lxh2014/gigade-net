var pageSize = 25;
Ext.define('gigade.EmailGroup', {
    extend: 'Ext.data.Model',
    fields: [
  
    { name: "group_id", type: "int" },
    { name: "group_name", type: "string" },
    { name: "group_createdate", type: "string" },
    { name: "group_updatedate", type: "string" },
    { name: "group_create_userid", type: "int" },
    { name: "group_update_userid", type: "int" },
    { name: "email_address", type: "string" },
    { name: "name", type: "string" },
    { name: "count", type: "int" },
     { name: "user_username", type: "string" },
    
    ]
});

EmailGroupStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EmailGroup',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/EmailGroupList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});

EmailGroupStore.on('beforeload', function () {
    Ext.apply(EmailGroupStore.proxy.extraParams,
    {
        group_name: Ext.getCmp('search_group_name').getValue(),
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("EmailGroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {

    var EmailGroup = Ext.create('Ext.grid.Panel', {
        id: 'EmailGroup',
        store: EmailGroupStore,
        flex: '8.9',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'group_id', width: 60, align: 'center' },
                     {
                         header: "功能", width: 120, align: 'center',
                         renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                             return "<a href='javascript:void(0);'onclick='Import(" + record.data.group_id + ")'>匯入</a>" + "&nbsp&nbsp&nbsp&nbsp&nbsp" + "<a href='javascript:void(0)' onclick='Export(" + record.data.group_id + ")'>匯出</a>";
                         }
                     },
        { header: "名稱", dataIndex: 'group_name', width: 150, align: 'center' },
        { header: "數量", dataIndex: 'count', width: 60, align: 'center' },
        { header: "更新時間", dataIndex: 'group_updatedate', width: 150, align: 'center' },
        { header: "更新人員", dataIndex: 'user_username', width: 150, align: 'center' },
       
        ],
        tbar: [
        { xtype: 'button', text: '新增', id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        { xtype: 'button', text: '編輯', id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
      
         '->',
         {
             xtype: 'textfield', fieldLabel: '關鍵字', id: 'search_group_name',LabelWidth:65,
             listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == e.ENTER) {
                         Search();
                     }
                 }
             }
         },
         {
             xtype: 'button', text: '查詢', handler: Search
         },
         {
             xtype: 'button', text: '重置', handler: function () {
                 Ext.getCmp('search_group_name').setValue();
             }
         },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EmailGroupStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "共計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [EmailGroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                EmailGroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //EdmContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, EmailGroupStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("EmailGroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
        editFunction(row[0], EmailGroupStore);
    }
}
function Search() {
    EmailGroupStore.removeAll();
    
    Ext.getCmp("EmailGroup").store.loadPage(1, {
        params: {
            group_name: Ext.getCmp('search_group_name').getValue(),
        }
    });
}

Import = function () {
    var row = Ext.getCmp("EmailGroup").getSelectionModel().getSelection();
    ImportFunction(row[0], EmailGroupStore);
}

 Export=function() {
     var row = Ext.getCmp("EmailGroup").getSelectionModel().getSelection();
     window.open('/EdmNew/ExportExcel?group_id='+row[0].data.group_id);
}

