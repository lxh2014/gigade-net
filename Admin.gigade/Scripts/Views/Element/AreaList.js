Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var CallidForm;
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.Users', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "area_id", type: "int" }, //
        { name: "show_number", type: "string" }, //
        { name: "area_name", type: "string" }, //
        { name: "area_desc", type: "string" }, //
        { name: "area_element_id", type: "string" }, //
        { name: "area_status", type: "string" }, //
        { name: "area_createdate", type: "string" }, //
        { name: "area_updatedate", type: "string" }, //
        { name: "create_userid", type: "string" }, //
        { name: "update_userid", type: "string" },
        { name: "element_type", type: "string" }
    ]
});

var ElementStores = Ext.create('Ext.data.Store', {
    //  autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Users',
    proxy: {
        type: 'ajax',
        url: '/Element/PageAreaList',
        reader: {
            type: 'json',
            root: 'data',//在執行成功后。顯示數據。所以record.data.用戶字段可以直接讀取
            totalProperty: 'totalCount'
        }
    }
    //    ,
    //      autoLoad: true
});



ElementStores.on('beforeload', function () {
    Ext.apply(ElementStores.proxy.extraParams, {
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        search_type: Ext.getCmp('search_type').getValue()
    });
});


var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdUser").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
//var channelTpl = new Ext.XTemplate(//Gwjserch

//           //'<a href="/Element/DetailIndex?area_id={area_id}">' + MORECONTENT + '</a>'
//        '<tpl if="this.canCopy(area_id)">',
//            '<a href="/Element/DetailIndex?area_id={area_id}">{area_id}</a>',
//        '</tpl>',
//         {
//             canCopy: function (area_id) {
//                 return area_id >= 0;
//             }
//         }

//);

function Query(x) {
    if ((Ext.getCmp("search_type").getValue() != "" && Ext.getCmp("search_type").getValue() != null) || Ext.getCmp("serchcontent").getValue() != "") {
        ElementStores.removeAll();
        Ext.getCmp("gdUser").store.loadPage(1, {
            params: {
                serchcontent: Ext.getCmp('serchcontent').getValue(),
                search_type: Ext.getCmp('search_type').getValue()
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
        store: ElementStores,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: BANNERID, dataIndex: 'area_id', width: 60, align: 'center' },
            { header: AREANAME, dataIndex: 'area_name', width: 150, align: 'center' },
            {
                header: "元素類型", dataIndex: 'element_type', width: 80, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "1") {
                        return "圖片";
                    } else if (value == "2") {
                        return "文字/跑馬燈";
                    }
                    else if (value == "3") {
                        return "商品";
                    }
                }
            },
             { header: SHOWNUMBER, dataIndex: 'show_number', width: 80, align: 'center' },
            { header: AREADESCRIBE, dataIndex: 'area_desc', width: 200, align: 'center' },
            { header: AREAID, dataIndex: 'area_element_id', width: 100, align: 'center' },
            { header: BANNERCREATE, dataIndex: 'area_createdate', width: 150, align: 'center' },
            { header: BANNERUPDATE, dataIndex: 'area_updatedate', width: 150, align: 'center' },

             {
                 header: BANNERSTATUS,
                 dataIndex: 'area_status',
                 id: 'con_status',
                 // hidden: true,
                 width: 60,
                 align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.area_id + ")'><img hidValue='0' id='img" + record.data.area_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.area_id + ")'><img hidValue='1' id='img" + record.data.area_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
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
                   fieldLabel: '元素類型',
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
            },
             {
                 text: RESET,
                 id: 'btn_reset',
                 listeners: {
                     click: function () {
                         Ext.getCmp("serchcontent").setValue("");
                         Ext.getCmp("search_type").setValue("");
                         // Query(1);
                     }
                 }
             }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: ElementStores,
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
    //ElementStores.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //    addWin.show();
    editFunction(null, ElementStores);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdUser").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], ElementStores);
    }
}

function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Element/UpdatePageAreaActive",
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
                        search_type: Ext.getCmp('search_type').getValue()

                    }
                });
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                Ext.getCmp("gdUser").store.load({
                    params: {
                        serchcontent: Ext.getCmp('serchcontent').getValue(),
                        search_type: Ext.getCmp('search_type').getValue()
                    }
                });
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, EDITERROR);
        }
    });
}
