var pageSize = 25;
Ext.define('gigade.PromoCarnet', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "event_name", type: "string" },
        { name: "event_desc", type: "string" },
        { name: "message_mode", type: "int" },
        { name: "message_content", type: "string" },
         { name: "group_id", type: "int" },
              { name: "group_name", type: "string" },
         { name: "link_url", type: "string" },
         { name: "promo_image", type: "string" },
         { name: "event_id", type: "string" },
         { name: "device", type: "string" },
         { name: "count_by", type: "int" },
          { name: "count", type: "int" },
         { name: "active_now", type: "string" },
           { name: "new_user", type: "string" },
            { name: "new_user_date", type: "string" },
             { name: "start", type: "string" },
              { name: "end", type: "string" },
              { name: "active", type: "int" },
               { name: "kuser", type: "int" },
                { name: "muser", type: "int" },
                { name: "created", type: "string" },
                 { name: "modified", type: "string" },
                   { name: "s_promo_image", type: "string" },
                     { name: "isSame", type: "string" },
                       { name: "present_event_id", type: "string" }
                     
       
    ]
});

var PromoCarnetStore = Ext.create('Ext.data.Store', {
    model: 'gigade.PromoCarnet',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/NewPromo/GetNewPromoCarnetList',
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
            var row = Ext.getCmp("PromoCarnet").getSelectionModel().getSelection();
            Ext.getCmp("PromoCarnet").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("PromoCarnet").down('#delete').setDisabled(selections.length == 0);
        }
    }
});
PromoCarnetStore.on('beforeload', function () {
    Ext.apply(PromoCarnetStore.proxy.extraParams, {
        condition: Ext.getCmp('ddlSel').getValue()
    })
})
function Query(x) {
    PromoCarnetStore.removeAll();
    Ext.getCmp("PromoCarnet").store.loadPage(1, {
        params: {
            condition: Ext.getCmp('ddlSel').getValue()

        }
    })
}
Ext.onReady(function () {
    var PromoCarnet = Ext.create('Ext.grid.Panel', {
        id: 'PromoCarnet',
        store: PromoCarnetStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
                        { header: EVENTID, dataIndex: 'event_id', width: 70, align: 'center' },
                        { header: GIFTIDNAME, dataIndex: 'present_event_id', width: 70, align: 'center' },
                        { header: EVENTNAME, dataIndex: 'event_name', width: 100, align: 'center' },
                        { header: EVENTDESC, dataIndex: 'event_desc', width: 150, align: 'center' },
                        {
                            header: MSGMODE, dataIndex: 'message_mode', width: 60, align: 'center',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == 0) {
                                    return MSG;
                                } else if (value == 1) {
                                    return SECRET;
                                }
                            }
                        },
                        { header: CONTENT, dataIndex: 'message_content', width: 150, align: 'center' },
                        {
                            header: USERGROUP, dataIndex: 'group_name', width: 100, align: 'center'
                        },
                        {
                            header: EVENTURL, dataIndex: 'link_url', width: 150, align: 'center',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return "<a href='" + value + "' target='_blank'>" + value + " </a>"
                            }
                        },
                        {
                            header: EVENTIMAGE, dataIndex: 's_promo_image', width: 150, align: 'center', width: 60, xtype: "templatecolumn"
                            , tpl: '<a target="_blank" href="{link_url}" ><img width=50 name="tplImg"  height=50 src="{s_promo_image}" /></a>'
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
                            header: LIMITCONDI, dataIndex: 'count_by', width: 60, align: 'center',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == 1) {
                                    return ORDER;
                                } else if (value == 2) {
                                    return USERS;
                                }
                            }
                        },
                        { header: LIMITCOUNT, dataIndex: 'count', width: 60, align: 'center' },
                    //    { header: "當天啟用", dataIndex: 'active_now', width: 150, align: 'center' },
                     { header: LIMITNEWUSER, dataIndex: 'new_user', width: 68, align: 'center' },
                     //   { header: "何時之後註冊的會員", dataIndex: 'new_user_date', width: 150, align: 'center' },
                        { header: EVENTSTART, dataIndex: 'start', width: 135, align: 'center' },
                        { header: EVENTEND, dataIndex: 'end', width: 135, align: 'center' },
                    //    { header: "是否有效", dataIndex: 'active', width: 150, align: 'center' },
                       // { header: "建立人", dataIndex: 'kuser', width: 150, align: 'center' },
                      //  { header: "修改人", dataIndex: 'muser', width: 150, align: 'center' },
                     //   { header: "創建時間", dataIndex: 'created', width: 150, align: 'center' },
                    //    { header: "修改時間", dataIndex: 'modified', width: 150, align: 'center' }, 

                        {
                            header: FUNCTION,
                            dataIndex: 'row_id',
                            align: 'center',
                            width: 120,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value > 0) {
                                    return "<a href=javascript:ExportExeclUserMessage(\"" + record.data.event_id + "\")>" + EXPORT + "</a> ";
                                }
                                else {
                                    return "<a href=javascript:ExportExeclUserMessage(\"" + record.data.event_id + "\")>" + EXPORT + "</a> ";

                                }
                            }
                        },
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
            { xtype: 'button', text: ADD, id: 'add', iconCls: 'ui-icon ui-icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'ui-icon ui-icon-user-edit', disabled: true, handler: onEditClick },
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
            store: PromoCarnetStore,
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
        selModel: sm,
        //viewConfig: {
        //    forceFit: true,
        //    getRowClass: function (record, rowIndex, rowParams, store) {
        //        if (record.data.isSame == "否") {
        //            return 'ems_actual_type';//注意这里返回的是定义好的css类；列如：(.ppp_ddd_sss div{background-color:red})定义到你页面访问到的css文件里。
        //        }
        //    }
        //}
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [PromoCarnet],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                PromoCarnet.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    //PromoCarnetStore.load({ params: { start: 0, limit: 25 } });
});

//*********新增********//
onAddClick = function () {
    editFunction(null, PromoCarnetStore);
}

//*********編輯********//
onEditClick = function () {
    var row = Ext.getCmp("PromoCarnet").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, SELECTONE);
    } else {
        editFunction(row[0], PromoCarnetStore);
    }
}

//**************刪除****************//

RemoveClick = function () {
    var row = Ext.getCmp("PromoCarnet").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id;
                    if (i != row.length - 1) {
                        rowIDs += ',';
                    }
                }

                Ext.Ajax.request({
                    url: '/NewPromo/DeleteCarnet',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            PromoCarnetStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
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
            url: "/NewPromo/UpdateActiveCarnet",
            data: {
                "row_id": id,
                "active": activeValue
            },
            type: "POST",
            dataType: "json",
            success: function (msg) {
                PromoCarnetStore.load();
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