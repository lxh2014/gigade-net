var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
var statusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
          { 'txt': HIDE, 'value': '0' },
          { 'txt': SHOW, 'value': '1' }
    ]

});
//群組管理Model
Ext.define('gigade.AnnounceContent', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "announce_id", type: "int" },
        { name: "title", type: "string" },
        { name: "content", type: "string" },
        { name: "sort", type: "int" },
        { name: "status", type: "int" },
        { name: "type", type: "int" },
    { name: "type_name", type: "string" },
        { name: "creator", type: "int" },
        { name: "c_name", type: "string" },
        { name: "create_time", type: "int" },
        { name: "create_date", type: "string" },
        { name: "modifier", type: "int" },
        { name: "u_name", type: "string" },
        { name: "modifier_time", type: "int" },
        { name: "modify_date", type: "string" }
    ]
});



var AnnounceContentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.AnnounceContent',
    proxy: {
        type: 'ajax',
        url: '/Announce/GetAnnounceList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("AnnounceContentGrid").down('#edit').setDisabled(selections.length == 0);

        }
    }
});

AnnounceContentStore.on('beforeload', function () {
    Ext.apply(AnnounceContentStore.proxy.extraParams,
        {
            typeCon: Ext.getCmp('typecon').getValue(),
            statusCon: Ext.getCmp('statusCon').getValue(),
            searchCon: Ext.getCmp('search_con').getValue()
        });
});
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

//類型
var typestore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=announce_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

Ext.onReady(function () {
    var AnnounceContentGrid = Ext.create('Ext.grid.Panel', {
        id: 'AnnounceContentGrid',
        store: AnnounceContentStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            {
                header: ID, dataIndex: 'announce_id', width: 60, align: 'center'
            },
            {
                header: TITLE, dataIndex: 'title', width: 150, align: 'center'
            },
            {
                header: SORT, dataIndex: 'sort', width: 80, align: 'center'
            },
            {
                header: ANNOUNCETYPE, dataIndex: 'type_name', width: 80, align: 'center'
            },
            {
                header: STATUS, dataIndex: 'status', width: 150, align: 'center',
                renderer: function (value) {
                    if (value == 1) {
                        return SHOW;
                    }
                    else if (value == 0) {
                        return HIDE;
                    }
                }
            },
            {
                header: CREATOR, dataIndex: 'c_name', width: 150, align: 'center'
            },
         {
             header: CREATEDATE, dataIndex: 'create_date', width: 150, align: 'center'
         },
         {
             header: UPDATEUSER, dataIndex: 'u_name', width: 150, align: 'center'
         }, {
             header: UPDATEDATE, dataIndex: 'modify_date', width: 150, align: 'center'
         }

        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          '->',
          {
              xtype: 'combobox',
              fieldLabel: ANNOUNCETYPE,
              labelWidth: 45,
              id: 'typecon',
              name: 'typecon',
              width: 140,
              store: typestore,
              editable: false,
              allowBlank: true,
              displayField: 'parameterName',
              valueField: 'parameterCode'
          },
           {
               xtype: 'combobox',
               fieldLabel: STATUS,
               id: 'statusCon',
               name: 'statusCon',
               labelWidth: 45,
               width: 140,
               store: statusStore,
               editable: false,
               allowBlank: true,
               displayField: 'txt',
               valueField: 'value'
           },
        {
            xtype: 'textfield',
            fieldLabel: KEY,
            labelWidth: 60,
            id: 'search_con',
            name: 'search_con',
            width: 160
        },
               {
                   xtype: 'button',
                   text: SEARCH,
                   iconCls: 'icon-search',
                   handler: Query

               },
               {
                   xtype: 'button',
                   text: RESET,
                   iconCls: 'ui-icon ui-icon-reset',
                   handler: function () {
                       Ext.getCmp('typecon').setValue(null);
                       Ext.getCmp('statusCon').setValue(null);
                       Ext.getCmp('search_con').setValue('');
                   }
               }

        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AnnounceContentStore,
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
        items: [AnnounceContentGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                AnnounceContentGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //AnnounceContentStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, AnnounceContentStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("AnnounceContentGrid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], AnnounceContentStore);
    }
}

function Query() {
    AnnounceContentStore.removeAll();
    if (Ext.getCmp('typecon').getValue() != null || Ext.getCmp('statusCon').getValue() != null | Ext.getCmp('search_con').getValue().trim()!="") {
        Ext.getCmp('AnnounceContentGrid').store.loadPage(1, {
            params: {
                typeCon: Ext.getCmp('typecon').getValue(),
                statusCon: Ext.getCmp('statusCon').getValue(),
                searchCon: Ext.getCmp('search_con').getValue()
            }

        });
    }
    else {
        Ext.Msg.alert(INFORMATION,"請選擇搜索內容");

    }
}




