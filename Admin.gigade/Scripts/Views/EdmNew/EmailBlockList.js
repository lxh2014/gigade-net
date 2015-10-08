pageSize = 25;
Ext.define('gigade.EmailBlockList', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'email_address', type: 'string' },
    { name: 'block_reason', type: 'string' },
    { name: 'block_createdate', type: 'string' },
    { name: 'block_updatedate', type: 'string' },
    { name: 'block_create_userid', type: 'int' },
    { name: 'block_update_userid', type: 'int' },
    { name: 'user_name', type: 'string' }
    ]
});
var EmailBlockListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    autoLoad: true,
    pageSize: pageSize,
    model: 'gigade.EmailBlockList',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetEmailBlockList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
EmailBlockListStore.on('beforeload', function () {
    Ext.apply(EmailBlockListStore.proxy.extraParams,
        {
            //email: Ext.getCmp('emailAddress').getValue(),
        });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdList").down('#unblock').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: EmailBlockListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.8,
        columns: [
        { header: "郵箱位址", dataIndex: 'email_address', flex: 1, align: 'center' },
        { header: "擋信原因", dataIndex: 'block_reason', flex: 1, align: 'center' },
        { header: "擋信時間", dataIndex: 'block_createdate', flex: 1, align: 'center' },
        { header: "設定人員", dataIndex: 'user_name', flex: 2, align: 'center' }
        ],
        tbar: [
        {
            xtype: 'button',
            text: ADD,
            id: 'add',
            iconCls: 'icon-user-add',
            handler: onAddClick
        },
        {
            xtype: 'button',
            text: EDIT,
            id: 'edit',
            iconCls: 'icon-user-edit',
            disabled: true,
            handler: onEditClick
        },
        {
            xtype: 'button',
            text: '解除',
            id: 'unblock',
            iconCls: 'icon-user-remove',
            disabled: true,
            handler: onUnblockClick
        }, '->',
        //{
        //    xtype: 'textfield',
        //    fieldLabel: '郵箱位址',
        //    id: 'emailAddress',
        //    name: 'emailAddress',
        //    labelWidth:60,
        //    listeners: {
        //        specialkey: function (field, e) {
        //            if (e.getKey() == e.ENTER) {
        //                Query(1);
        //            }
        //        }
        //    }
        //},
       //{
       //    xtype: 'button',
       //    iconCls: 'ui-icon ui-icon-search-2',
       //    text: "查詢",
       //    handler: Query
       //},
       // {
       //     xtype: 'button',
       //     text: '重置',
       //     id: 'btn_reset',
       //     iconCls: 'ui-icon ui-icon-reset',
       //     listeners: {
       //         click: function () {
       //             Ext.getCmp('emailAddress').reset();
       //         }
       //     }
       // }       
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EmailBlockListStore,
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
        layout: 'vbox',
        items: [gdList],
        renderTo: Ext.getBody(),      
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
})
Query = function () {
    EmailBlockListStore.removeAll();
    if (Ext.getCmp('emailAddress').getValue() != "") {
        Ext.getCmp("gdList").store.loadPage(1, {
            params: {
                email: Ext.getCmp('emailAddress').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, '請輸入郵箱位址');
    }
}
onAddClick = function () {
    addFunction(EmailBlockListStore);
}
onEditClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        editFunction(row[0], EmailBlockListStore);
    }
}
onUnblockClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        unBlockFunction(row[0], EmailBlockListStore);
    }
}