var CallidForm;
var pageSize = 25;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.VoteMessage', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "message_id", type: "int" },//信息編號
        { name: "article_id", type: "int" },//活動編號
        { name: "article_title", type: "string" },//來源IP
        { name: "update_time", type: "string" },//修改時間
        { name: "update_name", type: "string" },//修改人
        { name: "ip", type: "string" },//來源IP
        
        { name: "message_content", type: "string" },//留言內容
        { name: "message_status", type: "int" }//留言狀態
       
    ]
});
//文章Model
Ext.define("gigade.VoteArticle", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "article_id", type: "string" },
        { name: "article_title", type: "string" }
    ]
});
var VoteArticleStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VoteArticle',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Vote/GetArticle",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var StatusStore = Ext.create('Ext.data.Store', {
    fields: ['status_name', 'status_id'],
    data: [
        { "status_name": "所有狀態", "status_id": "-1" },
        { "status_name": "已啟用", "status_id": "1" },
        { "status_name": "未啟用", "status_id": "0" }
    ]
});



var VoteMessageStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.VoteMessage',
    proxy: {
        type: 'ajax',
        url: '/Vote/GetVoteMessageList',
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
            var row = Ext.getCmp("VoteMessage").getSelectionModel().getSelection();
            Ext.getCmp("VoteMessage").down('#edit').setDisabled(selections.length == 0);
            //Ext.getCmp("VoteMessage").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

VoteMessageStore.on('beforeload', function () {

    Ext.apply(VoteMessageStore.proxy.extraParams, {
        message: Ext.getCmp('message').getValue(),
        article: Ext.getCmp('Article').getValue(),
        status:Ext.getCmp('vote_status').getValue()
    })

});
function Query(x) {
    VoteMessageStore.removeAll();
    Ext.getCmp("VoteMessage").store.loadPage(1);

}

Ext.onReady(function () {
    VoteArticleStore.load();
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 100,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
               {
                   xtype: 'fieldcontainer',
                   combineErrors: true,
                   layout: 'hbox',
                   items: [
                            {
                                xtype: 'combobox',
                                editable: false,
                                fieldLabel: "文章名稱",
                                labelWidth: 70,
                                id: 'Article',
                                store: VoteArticleStore,
                                displayField: 'article_title',
                                valueField: 'article_id',
                                emptyText: '請選擇文章標題',
                                queryMode: 'remote',//local
                                lastQuery: ''
                            }
                           ,
                           {
                               xtype: 'textfield',
                               id: 'message',
                               margin: '0 5px',
                               name: 'message',
                               fieldLabel: '留言內容',
                               labelWidth: 70,
                               listeners: {
                                   specialkey: function (field, e) {
                                       if (e.getKey() == Ext.EventObject.ENTER) {
                                           Query();
                                       }
                                   }
                               }
                               
                           }

                   ]
               },
               {
                   xtype: 'fieldcontainer',
                   combineErrors: true,
                   layout: 'hbox',
                   items: [
                       {
                           xtype: 'combobox',
                           allowBlank: true,
                           hidden: false,
                           id: 'vote_status',
                           name: 'vote_status',
                           store: StatusStore,
                           queryMode: 'local',
                        
                           labelWidth: 70,
                           //margin: '0 5px',
                           displayField: 'status_name',
                           valueField: 'status_id',
                           typeAhead: true,
                           forceSelection: false,
                           editable: false,
                           fieldLabel: "查詢狀態",
                           emptyText: '所有狀態',
                           value:-1
                       }
                   ]
               },
            
              {
                  xtype: 'fieldcontainer',
                  combineErrors: true,
                  layout: 'hbox',
                  items: [
                      {
                          xtype: 'button',
                          margin: '0 8 0 8',
                          iconCls: 'icon-search',
                          text: "查詢",
                          handler: Query
                      },
                      {
                          xtype: 'button',
                          text: '重置',
                          id: 'btn_reset',
                          iconCls: 'ui-icon ui-icon-reset',
                          listeners: {
                              click: function () {
                                  Ext.getCmp('Article').setValue(0);//訂單 / 細項 / 課程編號
                                  Ext.getCmp('vote_status').setValue(-1);
                                  Ext.getCmp('Article_id').setValue("");//開始時間--time_start--delivery_date
                                  //Ext.getCmp('end_time').setValue("");//結束時間--time_end--delivery_date
                              }
                          }
                      }
                  ]
              }
        ]
    });
    var VoteMessage = Ext.create('Ext.grid.Panel', {
        id: 'VoteMessage',
        store: VoteMessageStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                        { header: "留言編號", dataIndex: 'message_id', width: 60, align: 'center' },
                        { header: "文章編號", dataIndex: 'article_id', width: 150, align: 'center' },
                        { header: "文章標題", dataIndex: 'article_title', width: 150, align: 'center' },
                        { header: "留言內容", dataIndex: 'message_content', width: 150, align: 'center' },
                       
                        { header: "修改人", dataIndex: 'update_name', width: 150, align: 'center' },
                        { header: "修改時間", dataIndex: 'update_time', width: 150, align: 'center' },
                        { header: "來源IP", dataIndex: 'ip', width: 150, align: 'center' },
                       
                         {
                            header: "留言狀態",
                            dataIndex: 'message_status',
                             id: 'status',
                             align: 'center',
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 if (value == 1) {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.message_id + ")'><img hidValue='0' id='img" + record.data.message_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                                 } else {
                                     return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.message_id + ")'><img hidValue='1' id='img" + record.data.message_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                                 }
                             }
                       }
        ],
        tbar: [
            { xtype: 'button', text: "新增", id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick }
            //,
            //{ xtype: 'button', text: "刪除", id: 'delete', disabled: true, iconCls: 'icon-remove', handler: RemoveClick },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VoteMessageStore,
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
        }
          , selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, VoteMessage],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                VoteMessage.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    VoteMessageStore.load({ params: { start: 0, limit: 25 } });
});
//*********新增********//
onAddClick = function () {
    editFunction(null, VoteMessageStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("VoteMessage").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示", "未選中任何行!");
    }
    else if (row.length > 1) {
        Ext.Msg.alert("提示", "只能選擇一行!");
    } else {
        editFunction(row[0], VoteMessageStore);
    }
}

//**************刪除****************//

RemoveClick = function () {
    var row = Ext.getCmp("VoteMessage").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert("提示", "未選中任何行");
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format("確認刪除?", row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.message_id;
                    if (i != row.length - 1) {
                        rowIDs += ',';
                    }
                }

                Ext.Ajax.request({
                    url: '/Vote/DeleteVoteMessage',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert("提示", "刪除成功!");
                            VoteMessageStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert("提示", "刪除超時!");
                    }
                });
            }
        });
    }
}
function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。                               // 返回日期。
    d.setDate(d.getDate() + 1);
    return d;
}
//更改活動狀態(啟用前先檢查該活動是否具有有效贈品，是則啟用，否則提示請設定有效贈品)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");

    $.ajax({
        url: "/Vote/UpdateVoteMessageStatus",
        data: {
            "message_id": id,
            "message_status": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            VoteMessageStore.load();
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
            } else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });

}

