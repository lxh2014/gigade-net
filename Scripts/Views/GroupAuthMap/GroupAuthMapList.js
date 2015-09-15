
var CallidForm;
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Groups', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "content_id", type: "int" },
        { name: "group_id", type: "string" },
        { name: "groupName", type: "string"},
        { name: "table_name", type: "string" },
        { name: "table_alias_name", type: "string" },
        { name: "column_name", type: "string" },
        { name: "value", type: "string" },
        { name: "status", type: "int" }
    ]
});

var GroupMapStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Groups',
    proxy: {
        type: 'ajax',
        //url:controller/fangfaming
        url: '/GroupAuthMap/GroupAuthMapList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdGroups").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
//site
Ext.define("gigade.fGroup", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "string" },
        { name: "groupName", type: "string" }]
});

var fGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.fGroup',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/GroupAuthMap/TFGroupList",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});

//var vipUserTpl = new Ext.XTemplate(
//    '<a href="/VipUserGroup/VipUserGroupAddList?id={group_id}">{list}</a>'
//);
GroupMapStore.on('beforeload', function () {
    Ext.apply(GroupMapStore.proxy.extraParams,
        {
            group_id: Ext.getCmp('groupid').getValue(),
            table_name: Ext.getCmp('searchcontent').getValue()
        });
});
Ext.onReady(function () {
    var gdGroups = Ext.create('Ext.grid.Panel', {
        id: 'gdGroups',
        store: GroupMapStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: CONTENTID, dataIndex: 'content_id', width: 60, align: 'center' },
            { header: GROUPID, dataIndex: 'groupName', width: 150, align: 'center' },
            { header: TABLENAME, dataIndex: 'table_name', width: 150, align: 'center' },
            { header: TABLEALIASNAME, dataIndex: 'table_alias_name', width: 100, align: 'center' },
            { header: COLUMNNAME, dataIndex: 'column_name', width: 100, align: 'center'},
            { header: VALUE, dataIndex: 'value', width: 100, align: 'center'},
            {
                header: STATUS, dataIndex: 'status', width: 70, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == 1) {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.content_id + ")'><img hidValue='0' id='img" + record.data.content_id + "' src='../../../Content/img/icons/accept.gif'/></a>";

                    }
                    else {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.content_id + ")'><img hidValue='1' id='img" + record.data.content_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                    }
                }
            }
        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
           '->',
           {
               xtype: 'combobox', allowBlank: true, id: 'groupid', name: 'groupid', fieldLabel: GROUPID, store: fGroupStore, emptyText: SELECT,
               displayField: 'groupName',
               valueField: 'rowid',
               typeAhead: true,
               forceSelection: false,
               editable: false,
               labelWidth: 40,
               value: 0
           },
           { xtype: 'textfield', allowBlank: true, id: 'searchcontent', name: 'searchcontent', fieldLabel: TABLENAME, labelWidth: 40 },
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
                      Ext.getCmp("groupid").setValue("");
                      Ext.getCmp("searchcontent").setValue("");
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: GroupMapStore,
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
        items: [gdGroups],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdGroups.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    GroupMapStore.load({ params: { start: 0, limit: 25 } });
});

function Query(x) {
    GroupMapStore.removeAll();
    Ext.getCmp("gdGroups").store.loadPage(1, {
        params: {
            group_id: Ext.getCmp('groupid').getValue(),
            table_name: Ext.getCmp('searchcontent').getValue()
        }
    });
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, GroupMapStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdGroups").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], GroupMapStore);
    }
}

//更改狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/GroupAuthMap/UpStatus",
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
                GroupMapStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                GroupMapStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            GroupMapStore.load();
        }
    });
}





