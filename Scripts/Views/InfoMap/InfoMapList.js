
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/


Ext.define('gigade.InfoMap', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "map_id", type: "int" },
        { name: "site_id", type: "string" },
        { name: "page_id", type: "string" },
        { name: "area_id", type: "string" },
        { name: "info_id", type: "string" },
        { name: "info_name", type: "string" },
        { name: "site_name", type: "string" },
        { name: "page_name", type: "string" },
        { name: "area_name", type: "string" },
        { name: "type", type: "int" },//類型 1：活動頁面 2：最新消息 3：訊息公告 4：電子報',
        { name: "sort", type: "int" },
        { name: "create_date", type: "string" },
        { name: "update_date", type: "string" },
        { name: "update_user_name", type: "string" }

    ]
});
var InfoMapStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.InfoMap',
    proxy: {
        type: 'ajax',
        url: '/InfoMap/GetInfoMapList',
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
            Ext.getCmp("gdInfo").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

InfoMapStore.on('beforeload', function () {
    Ext.apply(InfoMapStore.proxy.extraParams,
        {
            siteName: Ext.getCmp('siteName').getRawValue(),
            searchType: Ext.getCmp('search_type').getValue()
        });
});
Ext.define('gigade.InfoType', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "type_id", type: "string" },
    { name: "type_name", type: "string" }
    ]
});

var InfoTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.InfoType',
    autoLoad: true,
    data: [
        { type_id: '1', type_name: EPAPER },
        { type_id: '2', type_name: NEWS },
        { type_id: '3', type_name: ANNOUNCE },
        { type_id: '4', type_name: EDM }
    ]
});
Ext.onReady(function () {

    var gdInfo = Ext.create('Ext.grid.Panel', {
        id: 'gdInfo',
        store: InfoMapStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: MAPID, dataIndex: 'map_id', width: 60, align: 'center' },
            { header: SITE, dataIndex: 'site_name', width: 200, align: 'center' },
            { header: PAGE, dataIndex: 'page_name', width: 150, align: 'center' },
            { header: AREA, dataIndex: 'area_name', width: 200, align: 'center' },
            {
                header: TYPE, dataIndex: 'type', width: 200, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    switch (value) {
                        case 1:
                            return EPAPER;
                            break;
                        case 2:
                            return NEWS;
                            break;
                        case 3:
                            return ANNOUNCE;
                            break;
                        case 4:
                            return EDM;
                            break;

                    }

                }
            },
            { header: INFONAME, dataIndex: 'info_name', width: 200, align: 'center' },
            { header: "排序", dataIndex: 'sort', width: 150, align: 'center' },
            { header: "創建時間", dataIndex: 'create_date', width: 150, align: 'center' },
            { header: "更改時間", dataIndex: 'update_date', width: 150, align: 'center' },
            { header: "更改人", dataIndex: 'update_user_name', width: 150, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
            {
                xtype: 'combobox', //元素類型
                allowBlank: true,
                fieldLabel: TYPE,
                labelWidth: 80,
                editable: false,
                id: 'search_type',
                name: 'search_type',
                store: InfoTypeStore,
                displayField: 'type_name',
                valueField: 'type_id',
                typeAhead: true,
                width: 220,
                forceSelection: false,
                emptyText: SELECT,
                queryMode: 'local'
            },
           { xtype: 'textfield', fieldLabel: SITE, id: 'siteName', labelWidth: 65 },
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
                       Ext.getCmp("search_type").setValue(null);

                   }
               }
           }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: InfoMapStore,
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
        items: [gdInfo],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdInfo.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //InfoMapStore.load({ params: { start: 0, limit: 25 } });
});
/*********搜索***********/
function Query(x) {
    InfoMapStore.removeAll();
    if (Ext.getCmp('siteName').getRawValue() != "" || Ext.getCmp('search_type').getValue() != null) {
        Ext.getCmp("gdInfo").store.loadPage(1, {
            params: {
                siteName: Ext.getCmp('siteName').getRawValue(),
                searchType: Ext.getCmp('search_type').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION,"請選擇搜索條件");
    }
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, InfoMapStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdInfo").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], InfoMapStore);
    }
}





