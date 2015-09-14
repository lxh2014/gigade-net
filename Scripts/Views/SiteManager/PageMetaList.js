var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.PageMeta', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "pm_id", type: "int" },
        { name: "pm_url_para", type: "string" },
        { name: "pm_page_name", type: "string" },
        { name: "pm_title", type: "string" },
        { name: "pm_keywords", type: "string" },
        { name: "pm_description", type: "string" },
        { name: "pm_created", type: "string" },
        { name: "pm_modified", type: "string" },
        { name: "pm_modify_user", type: "int" },
        { name: "pm_create_user", type: "int" }
    ]
});
//到Controller獲取數據
var PageMetaStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.PageMeta',
    proxy: {
        type: 'ajax',
        url: '/SiteManager/GetPageMetaList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
//勾選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdMeta").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdMeta").down('#remove').setDisabled(selections.length == 0);
        }
    }
});

PageMetaStore.on('beforeload', function () {
    Ext.apply(PageMetaStore.proxy.extraParams, {
        search_content: Ext.getCmp('search_content') == null ? "" : Ext.getCmp('search_content').getValue()
    });
});
function Query() {
    if (Ext.getCmp('search_content').getValue() != "") {
        PageMetaStore.removeAll();
        Ext.getCmp("gdMeta").store.loadPage(1, {
            params: {
                params: { start: 0, limit: pageSize }
            }

        }); 
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }

}
Ext.onReady(function () {
    var gdMeta = Ext.create('Ext.grid.Panel', {
        id: 'gdMeta',
        store: PageMetaStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [

            { header: "名稱", dataIndex: 'pm_page_name', width: 150, align: 'center' },
            { header: "title", dataIndex: 'pm_title', width: 260, align: 'center' },
            { header: "url參數", dataIndex: 'pm_url_para', width: 260, align: 'center' },
            { header: "keywords", dataIndex: 'pm_keywords', width: 180, align: 'center' },
            { header: "description", dataIndex: 'pm_description', flex: 1, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
            '->',
         {
             xtype: 'textfield', fieldLabel: "名稱/title/keywords", labelWidth: 150, id: 'search_content', listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == e.ENTER) {
                         Query();
                     }
                 }
             }
         },
         {
             text: SEARCH,
             iconCls: 'icon-search',
             id: 'btnQuery',
             handler: Query
         }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: PageMetaStore,
            pageSize: pageSize,
            displayInfo: true,
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

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdMeta],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdMeta.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // PageMetaStore.load({ params: { start: 0, limit: pageSize } });
});

/********************************************新增*****************************************/
onAddClick = function () {
    editFunction(null);
}

/*********************************************編輯***************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdMeta").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0]);
    }
}


onRemoveClick = function () {
    var row = Ext.getCmp("gdMeta").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.pm_id + ',';
                }
                Ext.Ajax.request({
                    url: '/SiteManager/DeleteEdmPageMeta',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            PageMetaStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            PageMetaStore.load();
                        }
                    }
                });
            }
        });
    }
}