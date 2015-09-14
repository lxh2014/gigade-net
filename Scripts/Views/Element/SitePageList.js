/* 
* 文件名稱 :SitePageList.js 
* 文件功能描述 :頁面管理畫面 
* 版權宣告 : 
* 開發人員 : changjian0408j 
* 版本資訊 : 1.0 
* 日期 : 2014/10/14 
* 修改人員 : 
* 版本資訊 : 
* 日期 : 
* 修改備註 : 
*/
var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.SitePageList', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "page_id", type: "int" },
        { name: "page_name", type: "string" },
        { name: "page_url", type: "string" },
        { name: "page_status", type: "int" },
        { name: "page_html", type: "string" },
        { name: "page_desc", type: "string" },
        { name: "page_createdate", type: "string" },
        { name: "page_updatedate", type: "string" },
        { name: "create_userid", type: "int" },
        { name: "update_userid", type: "int" }
    , { name: 'page_shortHtml', type: 'string' }

    ]
});



var SitePageListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.SitePageList',
    proxy: {
        type: 'ajax',
        url: '/Element/SitePageList',
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
            //Ext.getCmp("SitePageListGrid").down('#add').setDisabled(selections.length == 0);
            Ext.getCmp("SitePageListGrid").down('#edit').setDisabled(selections.length == 0);

        }
    }
});

SitePageListStore.on('beforeload', function () {
    Ext.apply(SitePageListStore.proxy.extraParams,
        {
            pagename: Ext.getCmp('pagename').getRawValue()
        });
});

Ext.onReady(function () {
    var SitePageListGrid = Ext.create('Ext.grid.Panel', {
        id: 'SitePageListGrid',
        store: SitePageListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,

        frame: true,
        columns: [
            {
                header: PAGEID, dataIndex: 'page_id', width: 60, align: 'center'
            },
            {
                header: PAGENAME, dataIndex: 'page_name', width: 150, align: 'center'
            },
            {
                header: PAGEURL, dataIndex: 'page_url', width: 200, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == " ") {
                        return "";
                    }
                    else {

                        return Ext.String.format("<a  href='{0}' target='_blank'>{1}</a>", record.data.page_url, record.data.page_url);
                    }
                }
            },

            {
                header: PAGEHTML, dataIndex: 'page_shortHtml', width: 200, align: 'center'
            },
            { header: PAGEDESC, dataIndex: 'page_desc', width: 150, align: 'center' },
            { header: PAGECREATEDATE, dataIndex: 'page_createdate', width: 150, align: 'center' },
            { header: PAGEUPDATEDATE, dataIndex: 'page_updatedate', width: 150, align: 'center' },
              {
                  header: PAGESTATUS, dataIndex: 'page_status', width: 60, align: 'center',
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      if (value == 1) {
                          return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.page_id + ")'><img hidValue='0' id='img" + record.data.page_id + "' src='../../../Content/img/icons/accept.gif'/></a>";

                      }
                      else {
                          return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.page_id + ")'><img hidValue='1' id='img" + record.data.page_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                      }
                  }
              },

        ],
        tbar: [
           { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
           { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        '->',
         {
             xtype: 'textfield', fieldLabel: PAGENAME, id: 'pagename', labelWidth: 65, listeners: {
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
                         Ext.getCmp("pagename").setValue("");
                         //Query(1);

                     }
                 }
             }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SitePageListStore,
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
        items: [SitePageListGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                SitePageListGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    // SitePageListStore.load({ params: { start: 0, limit: 25 } });
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {

    //addWin.show();
    editFunction(null, SitePageListStore);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("SitePageListGrid").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], SitePageListStore);
    }
}

//更改頁面狀態(啟用或者禁用)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    //alert(activeValue);
    $.ajax({
        url: "/Element/UpdateSitePageStatus",
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
                SitePageListStore.load();
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                SitePageListStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            SitePageListStore.load();
        }
    });
}
//查詢
function Query(x) {
    if (Ext.getCmp("pagename").getValue() != "") {
        SitePageListStore.removeAll();
        Ext.getCmp("SitePageListGrid").store.loadPage(1, {
            params: {
                pagename: Ext.getCmp('pagename').getRawValue()
            }
        });
    }
    else {
        Ext.Msg.alert(INFORMATION, SEARCH_LIMIT);
    }


}






