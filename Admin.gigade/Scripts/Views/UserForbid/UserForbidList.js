var pageSize = 20;
/**********************************************************************群組管理主頁面**************************************************************************************/
//自定义VTypes类型，验证日期范围  
Ext.apply(Ext.form.VTypes, {
    dateRange: function (val, field) {
        if (field.dateRange) {
            var beginId = field.dateRange.begin;
            this.beginField = Ext.getCmp(beginId);
            var endId = field.dateRange.end;
            this.endField = Ext.getCmp(endId);
            var beginDate = this.beginField.getValue();
            var endDate = this.endField.getValue();
        }
        if (beginDate != null && endDate != null) {
            if (beginDate <= endDate) {
                return true;
            } else {
                return false;
            }
        }
        else {
            return true;
        }

    },
    //验证失败信息  
    dateRangeText: '開始時間不能大於結束時間'
});
//群組管理Model
Ext.define('gigade.UserForbid', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "forbid_id", type: "int" },
        { name: "forbid_ip", type: "string" },
        { name: "forbid_createdate", type: "string" },
        { name: "forbid_createuser", type: "int" },
        { name: "create_time", type: "string" },
        { name: "user_username", type: "string" }

    ]
});
//到Controller獲取數據
var UserForbidStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.UserForbid',
    proxy: {
        type: 'ajax',
        url: '/UserForbid/GetUserForbidList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
//勾選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdUserForbid").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdUserForbid").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
UserForbidStore.on('beforeload', function () {
    Ext.apply(UserForbidStore.proxy.extraParams, {
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        timestart: Ext.getCmp('timestart').getValue(),
        timeend: Ext.getCmp('timeend').getValue()
    });
});
function Query(x) {
    UserForbidStore.removeAll();
    Ext.getCmp("gdUserForbid").store.loadPage(1, {
        params: {
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            timestart: Ext.getCmp('timestart').getValue(),
            timeend: Ext.getCmp('timeend').getValue()
        }
    });
}
Ext.onReady(function () {
    var gdUserForbid = Ext.create('Ext.grid.Panel', {
        id: 'gdUserForbid',
        store: UserForbidStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: '流水號', dataIndex: 'forbid_id', width: 70, align: 'center' },
            { header: '禁用IP', dataIndex: 'forbid_ip', width: 120, align: 'center' },
            { header: '創建時間', dataIndex: 'create_time', width: 120, align: 'center' },
            { header: '創建人', dataIndex: 'user_username', width: 130, align: 'center' }
        ],

        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: '刪除', id: 'remove', hidden: false, disabled: true, iconCls: 'icon-user-remove', handler: onRemoveClick },
             '->',
                              {
                                  xtype: 'datefield',
                                  allowBlank: true,
                                  id: 'timestart',
                                  margin: "0 5 0 0",
                                  editable: false,
                                  value: new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()),
                                  name: 'serchcontent',
                                  fieldLabel: '創建時間',
                                  labelWidth: 60,
                                  width:170,
                                  editable: false,
                                  listeners: {
                                      select: function (a, b, c) {
                                          var tstart = Ext.getCmp("timestart");
                                          var tend = Ext.getCmp("timeend");
                                          if (tend.getValue() == null) {
                                              tend.setValue(setNextMonth(tstart.getValue(), 1));
                                          }
                                          else if (tend.getValue() < tstart.getValue()) {
                                              Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                              tend.setValue(setNextMonth(tstart.getValue(), 1));
                                          }
                                      }
                                  }
                              },
             {
                 xtype: 'datefield',
                 allowBlank: true,
                 editable: false,
                 id: 'timeend',
                 margin: "0 5 0 0",
                 name: 'serchcontent',
                 fieldLabel: '到',
                 width: 125,
                 value:new Date(),
                 labelWidth: 15,
                 listeners: {
                     select: function (a, b, c) {
                         var tstart = Ext.getCmp("timestart");
                         var tend = Ext.getCmp("timeend");
                         if (tstart.getValue() == null) {
                             tstart.setValue(setNextMonth(tend.getValue(), -1));
                         }
                         else if (tend.getValue() < tstart.getValue()) {
                             Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                             tstart.setValue(setNextMonth(tend.getValue(), -1));
                         }
                     }
                 }
             },
             {
                 xtype: 'textfield', fieldLabel: "查詢IP", id: 'serchcontent', labelWidth: 55, listeners: {
                     specialkey: function (field, e) {
                         // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                         // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                         if (e.getKey() == e.ENTER) {
                             Query(1);
                         }
                     }
                 }
             },
            {
                text: "查詢",
                iconCls: 'icon-search',
                id: 'btnQuery',
                hidden: false,
                handler: Query
            },
            {
                xtype: 'button',
                text: '重置',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp('serchcontent').setValue("");
                        Ext.getCmp('timestart').setValue(new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()));
                        Ext.getCmp('timeend').setValue(new Date());
                        Query();
                    }
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: UserForbidStore,
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
        items: [gdUserForbid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdUserForbid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //UserForbidStore.load({ params: { start: 0, limit: pageSize } });
});

/********************************************新增*****************************************/
onAddClick = function () {
    editFunction(null, UserForbidStore);
}

/*********************************************編輯***************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdUserForbid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], UserForbidStore);
    }
}

onRemoveClick = function () {
    var row = Ext.getCmp("gdUserForbid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.forbid_id + ',';
                }
                Ext.Ajax.request({
                    url: '/UserForbid/DeleteUserFoid',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            UserForbidStore.load();
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
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}


