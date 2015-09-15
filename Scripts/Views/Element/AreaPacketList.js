Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
/**********************************************************************區域包主頁面**************************************************************************************/
Ext.define("gigade.ElementType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});
var ElementTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ElementType',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Element/GetElementType",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "packet_id", type: "int" }, //
        { name: "packet_name", type: "string" }, //
        { name: "show_number", type: "int" }, //
        { name: "packet_sort", type: "int" }, //
        { name: "element_type", type: "int" }, //
        { name: "packet_status", type: "int" }, //
        { name: "packet_desc", type: "string" }, //
        { name: "packet_createdate", type: "string" }, //
        { name: "packet_updatedate", type: "string" }, //
        { name: "create_userid", type: "int" },
        { name: "update_userid", type: "int" }
    ]
});

var AreaPacketStores = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Element/AreaPacketList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
});



AreaPacketStores.on('beforeload', function () {
    Ext.apply(AreaPacketStores.proxy.extraParams, {
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        serchtype: Ext.getCmp('ele_type').getValue()
    });
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdUser").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    if ((Ext.getCmp("ele_type").getValue() != null && Ext.getCmp("ele_type").getValue() != "") || Ext.getCmp("serchcontent").getValue() != "") {
        AreaPacketStores.removeAll();
        Ext.getCmp("gdUser").store.loadPage(1, {
            params: {
                serchcontent: Ext.getCmp('serchcontent').getValue(),
                serchtype: Ext.getCmp('ele_type').getValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }

}
Ext.onReady(function () {
    var gdUser = Ext.create('Ext.grid.Panel', {
        id: 'gdUser',
        store: AreaPacketStores,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: BANNERID, dataIndex: 'packet_id', width: 60, align: 'center' },
            { header: PACKETNAME, dataIndex: 'packet_name', width: 150, align: 'center' },
            {
                header: ELEMENTTYPE, dataIndex: 'element_type', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return IMAGE;
                    } else if (value == "2") {
                        return TEXT;
                    }
                    else if (value == "3") {
                        return PROD;
                    }
                }
            },
            { header: SHOWNUMBER, dataIndex: 'show_number', width: 80, align: 'center' },
            { header: SORT, dataIndex: 'packet_sort', width: 80, align: 'center' },

            { header: ESCINFO, dataIndex: 'packet_desc', width: 100, align: 'center' },
              {
                  header: FUNTION,
                  dataIndex: 'packet_id',
                  align: 'center',
                  width: 180,
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      return '<a href=javascript:TranToDetial("/Element/DetailIndex","' + value + '")>' + CHILDELEMENT + '</a> ';

                  }
              }
               ,
            { header: BANNERCREATE, dataIndex: 'packet_createdate', width: 150, align: 'center' },
            { header: BANNERUPDATE, dataIndex: 'packet_updatedate', width: 150, align: 'center' },

             {
                 header: BANNERSTATUS,
                 dataIndex: 'packet_status',
                 id: 'packet_status',
                 // hidden: true,
                 width: 60,
                 align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.packet_id + ")'><img hidValue='0' id='img" + record.data.packet_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.packet_id + ")'><img hidValue='1' id='img" + record.data.packet_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        tbar: [
             { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
             '->',
             {
                 xtype: 'combobox',
                 fieldLabel: ELEMENTTYPE,
                 id: 'ele_type',
                 name: 'ele_type',
                 labelWidth: 80,
                 editable: false,
                 typeAhead: true,
                 queryModel: 'local',
                 forceSelection: false,
                 store: ElementTypeStore,
                 displayField: 'parameterName',
                 valueField: 'ParameterCode',
                 emptyText: SELECT,
                 listeners: {
                     change: function (newValue, oldValue, e) {
                         if (newValue) {
                             if (Ext.getCmp("ele_type").getValue() != "") {
                                 Query(1);
                             }
                         }
                     }
                 }
             },
             {
                 xtype: 'textfield', fieldLabel: KEY, id: 'serchcontent', labelWidth: 80, listeners: {
                     specialkey: function (field, e) {
                         if (e.getKey() == e.ENTER) {
                             Query(1);
                         }
                     }
                 }
             },
            {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                hidden: false,
                handler: Query
            }
            ,
             {
                 text: RESET,
                 id: 'btn_reset',
                 listeners: {
                     click: function () {
                         Ext.getCmp("serchcontent").setValue("");
                         Ext.getCmp("ele_type").setValue("");
                         // Query(1);
                     }
                 }
             }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: AreaPacketStores,
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
        items: [gdUser],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdUser.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // AreaPacketStores.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, AreaPacketStores);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], AreaPacketStores);
    }
}

function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");

    $.ajax({
        url: "/Element/UpdateAreaPacketActive",
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
                Ext.getCmp("gdUser").store.load({
                    params: {
                        serchcontent: Ext.getCmp('serchcontent').getValue(),
                        serchtype: Ext.getCmp('ele_type').getValue()
                    }
                });
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                Ext.getCmp("gdUser").store.load({
                    params: {
                        serchcontent: Ext.getCmp('serchcontent').getValue(),
                        serchtype: Ext.getCmp('ele_type').getValue()
                    }
                });
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, EDITERROR);
        }
    });

}

function TranToDetial(url, packet_id) {
    var record = ELEMENTDETAIL;
    var urlTran = url + '?packet_id=' + packet_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#eledetial');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'eledetial',
        title: record,
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();

}