var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Sites', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "site_id", type: "int" },
        { name: "site_name", type: "string" },
        { name: "domain", type: "string" },
        { name: "cart_delivery", type: "string" },
        { name: "csitename", type: "string" },
        { name: "online_user", type: "int" },
        { name: "max_user", type: "int" },
        { name: "page_location", type: "string" },
        { name: "site_status", type: "string" },
        { name: "site_createdate", type: "string" },
        { name: "site_updatedate", type: "string" },
        { name: "create_userid", type: "string" },
        { name: "update_userid", type: "string" }
    ]
});

var SitesStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Sites',
    proxy: {
        type: 'ajax',
        url: '/Element/SiteList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

SitesStore.on('beforeload', function () {
    Ext.apply(SitesStore.proxy.extraParams,
        {
            serchcontent: Ext.getCmp('serchcontent').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdSites").down('#edit').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#remove').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#auth').setDisabled(selections.length == 0);
            //            Ext.getCmp("gdFgroup").down('#callid').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    if (Ext.getCmp("serchcontent").getValue() != "") {
        SitesStore.removeAll();
        Ext.getCmp("gdSites").store.loadPage(1, {
            params: {
                serchcontent: Ext.getCmp('serchcontent').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }


}

Ext.onReady(function () {
    var gdSites = Ext.create('Ext.grid.Panel', {
        id: 'gdSites',
        store: SitesStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: SITEID, dataIndex: 'site_id', width: 60, align: 'center' },
            { header: SITENAME, dataIndex: 'site_name', width: 150, align: 'center' },
            { header: DOMAIN, dataIndex: 'domain', width: 200, align: 'center' },
            { header: SCATEDELIVERYID, dataIndex: 'cart_delivery', width: 150, align: 'center', hidden: true },
            { header: SCATEDELIVERY, dataIndex: 'csitename', width: 150, align: 'center' },
            { header: ONLINEUSER, dataIndex: 'online_user', width: 80, align: 'center' },
            { header: MAXUSER, dataIndex: 'max_user', width: 80, align: 'center' },
            { header: PAGELOCATION, dataIndex: 'page_location', width: 200, align: 'center' },
            { header: BANNERCREATE, dataIndex: 'site_createdate', width: 150, align: 'center' },
            { header: BANNERUPDATE, dataIndex: 'site_updatedate', width: 150, align: 'center' },
            {
                header: "狀態", dataIndex: 'site_status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.site_id + ")'><img hidValue='0' id='img" + record.data.site_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.site_id + ")'><img hidValue='1' id='img" + record.data.site_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
           {
               xtype: 'textfield', allowBlank: true, id: 'serchcontent', name: 'serchcontent', fieldLabel: SITENAME, labelWidth: 60, listeners: {
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
                      Ext.getCmp("serchcontent").setValue("");
                      //Query(1);
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SitesStore,
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
   // SitesStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, SitesStore);
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
        editFunction(row[0], SitesStore);
    }
}

//更改會員狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Element/UpdateSiteState",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                SitesStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                SitesStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            SitesStore.load();
        }
    });
}





