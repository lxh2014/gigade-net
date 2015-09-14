var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Sites', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "map_id", type: "int" },
        { name: "site_id", type: "string" },
        { name: "page_id", type: "string" },
        { name: "area_id", type: "string" },
        { name: "packet_id", type: "string" },
        { name: "site_name", type: "string" },
        { name: "page_name", type: "string" },
        { name: "area_name", type: "string" },
        { name: "packet_name", type: "string" },
        { name: "sort", type: "int" },
        { name: "create_date", type: "string" },
        { name: "update_date", type: "string" },
        { name: "create_user_name", type: "string" }

    ]
});
var ElementMapStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Sites',
    proxy: {
        type: 'ajax',
        url: '/Element/ElementMapList',
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
            Ext.getCmp("gdSites").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

ElementMapStore.on('beforeload', function () {
    Ext.apply(ElementMapStore.proxy.extraParams,
        {
            siteName: Ext.getCmp('siteName').getRawValue(),
            searchType: Ext.getCmp('search_type').getValue()
        });
});

Ext.onReady(function () {
    var gdSites = Ext.create('Ext.grid.Panel', {
        id: 'gdSites',
        store: ElementMapStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: BANNERID, dataIndex: 'map_id', width: 60, align: 'center' },
            { header: SITENAME, dataIndex: 'site_name', width: 200, align: 'center' },
            { header: PAGENAME, dataIndex: 'page_name', width: 150, align: 'center' },
            { header: AREANAME, dataIndex: 'area_name', width: 200, align: 'center' },
            { header: PACKETNAME, dataIndex: 'packet_name', width: 200, align: 'center' },
            { header: SORT, dataIndex: 'sort', width: 150, align: 'center' },
            { header: BANNERCREATE, dataIndex: 'create_date', width: 150, align: 'center' },
            { header: BANNERUPDATE, dataIndex: 'update_date', width: 150, align: 'center' },
            { header: UPDATEUSER, dataIndex: 'create_user_name', width: 150, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
            {
                xtype: 'combobox',
                fieldLabel: ELEMENTTYPE,
                id: 'search_type',
                name: 'search_type',
                labelWidth: 80,
                editable: false,
                typeAhead: true,
                queryModel: 'local',
                forceSelection: false,
                store: elementTypeStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                emptyText: SELECT,
                listeners: {
                    change: function (newValue, oldValue, e) {
                        if (newValue) {
                            if (Ext.getCmp("search_type").getValue() != "") {
                                Query(1);
                            }
                        }
                    }
                }
            },
            {
                xtype: 'textfield', fieldLabel: SITENAME, id: 'siteName', labelWidth: 65, listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query(1);
                        }
                    }
                }
            },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
           {
               xtype: 'button',
               text: RESET,
               id: 'btn_reset',
               listeners: {
                   click: function () {
                       Ext.getCmp("siteName").setValue("");
                       Ext.getCmp("search_type").setValue("");
                       // Query(1);

                   }
               }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ElementMapStore,
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
        items: [gdSites],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdSites.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // ElementMapStore.load({ params: { start: 0, limit: 25 } });
});
/*********搜索***********/
function Query(x) {
    if ((Ext.getCmp("search_type").getValue() != null && Ext.getCmp("search_type").getValue() != "") || Ext.getCmp("siteName").getValue() != "") {
        ElementMapStore.removeAll();
        Ext.getCmp("gdSites").store.loadPage(1, {
            params: {
                siteName: Ext.getCmp('siteName').getRawValue(),
                searchType: Ext.getCmp('search_type').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }

}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, ElementMapStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdSites").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ElementMapStore);
    }
}





