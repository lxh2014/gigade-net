var CallidForm;
Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var pageSize = 25;
//促銷贈品表Model
Ext.define('gigade.NewPromoQuestionnaire', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },//流水編號
        { name: "event_name", type: "string" },//活動名稱
        { name: "event_desc", type: "string" },//活動描述
        { name: "event_id", type: "string" },//促銷編號
        { name: "group_id", type: "int" },//會員群組
         { name: "group_name", type: "string" },
        { name: "link_url", type: "string" },//　	鏈接地址
        { name: "promo_image", type: "string" },//活動圖檔
        { name: "device", type: "string" },//　	裝置
        { name: "count_by", type: "string" },//次數限制依  1:訂單  2:會員
        { name: "count", type: "string" },//　	限制次數
        { name: "active_now", type: "bool" },//當天就啟用 0:否 1:是
        { name: "new_user", type: "bool" },//0	是否限制為新會員參加
        { name: "new_user_date", type: "string" },//何時之後註冊的會員
        { name: "start", type: "string" },//活動開始時間
        { name: "end", type: "string" },//活動結束時間
        { name: "active", type: "string" },//1	是否啟用0:無效  1:有效
        { name: "kuser", type: "string" },//　	建立人
        { name: "muser", type: "string" },//修改人
        { name: "created", type: "string" },//創建時間
        { name: "modified", type: "string" },//修改時間
         { name: "present_event_id", type: "string" },//修改時間
         { name: "s_promo_image", type: "string" }//圖片的絕對路徑

    ]
});

//
//促銷贈品表Store
var NewPromoQuestionnaireStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.NewPromoQuestionnaire',
    proxy: {
        type: 'ajax',
        url: '/NewPromo/PromoQuestionnaireList',
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

            Ext.getCmp("NewPromoQuestionnaire").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("NewPromoQuestionnaire").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
NewPromoQuestionnaireStore.on('beforeload', function () {
    Ext.apply(NewPromoQuestionnaireStore.proxy.extraParams, {
        SearchTime: Ext.getCmp('ddlSel').getValue()
    })

});


function Query(x) {
    NewPromoQuestionnaireStore.removeAll();
    Ext.getCmp("NewPromoQuestionnaire").store.loadPage(1);
}

Ext.onReady(function () {
    var NewPromoQuestionnaire = Ext.create('Ext.grid.Panel', {
        id: 'NewPromoQuestionnaire',
        store: NewPromoQuestionnaireStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: EVENTID, dataIndex: 'event_id', width: 80, align: 'center' },
                { header: "贈品編號", dataIndex: 'present_event_id', width: 70, align: 'center' },
            { header: EVENTNAME, dataIndex: 'event_name', width: 150, align: 'center' },
            { header: EVENTDESC, dataIndex: 'event_desc', width: 150, align: 'center' },
            {
                header: USERGROUP, dataIndex: 'group_name', width: 100, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                    if (value==""||value==null) {
                        return "不分";
                    } else {
                        return value;
                    }
                }
            },
            {
                header: EVENTURL, dataIndex: 'link_url', width: 120, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    return "<a href='" + value + "' target='_blank'>" + value + " </a>"
                }
            },
            {
                header: EVENTIMAGE, dataIndex: 's_promo_image', width: 150, align: 'center', width: 60,
                xtype: 'templatecolumn',
                tpl: '<a target="_blank" href="{link_url}" ><img width=50 name="tplImg"  height=50 src="{s_promo_image}" /></a>'
            },
            {
                header: DEVICE, dataIndex: 'device', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                    if (record.data.device == 0) {
                        return NOSELECT;
                    } else if (record.data.device == 1) {
                        return PC;
                    }
                    else if (record.data.device == 2) {
                        return PHONE;
                    }
                }
            },
            {
                header: LIMITCONDI, dataIndex: 'count_by', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                    if (value == 1) {
                        return ORDER;
                    } else if (value == 2) {
                        return USERS;
                    }
                }
            },
            { header: LIMITCOUNT, dataIndex: 'count', width: 60, align: 'center' },
            { header: ACTIVENEW, dataIndex: 'active_now', hidden: true, width: 60, align: 'center' },
            {
                header: LIMITNEWUSER, dataIndex: 'new_user', width: 80, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value) {
                        return YES;
                    } else {
                        return NO;
                    }
                }
            },
            { header: USERTIME, dataIndex: 'new_user_date', hidden: true, width: 145, align: 'center' },//hidden: true, 
            { header: EVENTSTART, dataIndex: 'start', width: 145, align: 'center' },
            { header: EVENTEND, dataIndex: 'end', width: 145, align: 'center' },

              {
                  header: FUNCTION,
                  dataIndex: 'row_id',
                  align: 'center',
                  width: 130,
                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                      if (value > 0) {
                          return "<a href=javascript:ExportExeclUserMessage(\"" + record.data.event_id + "\")>" + EXPORT + "</a>  ";
                      }
                      else {
                          return "<a href=javascript:ExportExeclUserMessage(\"" + record.data.event_id + "\")>" + EXPORT + "</a> ";

                      }
                  }
              }
              ,
               {
                   header: ACTIVE,
                   dataIndex: 'active',
                   id: 'controlactive',
                   align: 'center',
                   renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                       if (value == "1") {
                           return "<a href='javascript:void(0);' onclick='UpdateActive(\"" + record.data.row_id + "\")'><img hidValue='0' id='img" + record.data.row_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                       } else {
                           return "<a href='javascript:void(0);' onclick='UpdateActive(\"" + record.data.row_id + "\")'><img hidValue='1' id='img" + record.data.row_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                       }
                   }
               }
        ],
        tbar: [
          { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
          { xtype: 'button', text: EDIT, id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          { xtype: 'button', text: REMOVE, id: 'delete', iconCls: 'icon-remove', disabled: true, handler: RemoveClick },
           '->',
            {
                xtype: 'combobox', editable: false, fieldLabel: CONDITON, labelWidth: 60, id: 'ddlSel', store: DDLStore, displayField: 'txt', valueField: 'value', value: '1'
            }
            , {
                text: SEARCH,
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: NewPromoQuestionnaireStore,
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
        layout: 'fit',
        items: [NewPromoQuestionnaire],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                NewPromoQuestionnaire.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //NewPromoQuestionnaireStore.load({ params: { start: 0, limit: 25 } });
});
//*********新增********//
onAddClick = function () {
    editFunction(null, NewPromoQuestionnaireStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("NewPromoQuestionnaire").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else {
        editFunction(row[0], NewPromoQuestionnaireStore);
    }
}
//**************刪除****************//

RemoveClick = function () {
    var row = Ext.getCmp("NewPromoQuestionnaire").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id + ',';
                }

                Ext.Ajax.request({
                    url: '/NewPromo/DeleteQuestion',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        NewPromoQuestionnaireStore.load();

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        NewPromoQuestionnaireStore.load();
                    }
                });
            }
        });
    }
}

function TranToSetGift(eventId) {
    var url = '/NewPromo/NewPromoPresent?Event_Id=' + eventId;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#Sms');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'Sms',
        title: GIFTSET,
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}




//更改活動狀態(啟用前先檢查該活動是否具有有效贈品，是則啟用，否則提示請設定有效贈品)
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
        $.ajax({
            url: "/NewPromo/UpdateActiveQuestion",
            data: {
            "row_id": id,
                "active": activeValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                NewPromoQuestionnaireStore.load();
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

function ExportExeclUserMessage(id) {
    window.open("/NewPromo/ExportNewPromoRecordList?event_id=" + id);
}