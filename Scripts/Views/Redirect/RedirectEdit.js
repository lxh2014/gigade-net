
//會員群組Model
Ext.define("gigade.VipGroup", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "string" },
        { name: "group_name", type: "string" }]
});
//會員群組store
var VipGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipGroup',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Redirect/GetVipGroup",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
function editRedirectFunction(row, store)
{
    var editRedirectFrm = Ext.create('Ext.form.Panel', {
        id: 'editRedirectFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Redirect/SaveRedirect',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
        {
            xtype: 'displayfield',
            fieldLabel: '編號',
            id: 'redirect_id',
            name: 'redirect_id',
            editable: false,
            hidden: row == null ? true : false
        },
        {
            xtype: 'textfield',
            fieldLabel: "名稱",
            allowBlank: false,
            id: 'redirect_name',
            name: 'redirect_name'
        },
            {
                xtype: 'textfield',
                fieldLabel: "目的連結",
                name: 'redirect_url',
                id: 'redirect_url',
                labelWidth: 80,
                submitValue: true,
                vtype: 'url',
                allowBlank: false
            },
              {
                  xtype: 'combobox', //會員群組
                  editable: false,
                  hidden: false,
                  fieldLabel: '會員綁定群組',
                  id: 'user_group_id',
                  name: 'user_group_id',
                  hiddenName: 'user_group_id',
                  store: VipGroupStore,
                  lastQuery: '',
                  displayField: 'group_name',
                  valueField: 'group_id',
                  typeAhead: true,
                  forceSelection: false,
                  value: "0",
                  hidden:true
              },
              {

                  xtype: 'radiogroup',
                  fieldLabel: "狀態",
                  id: 'redirect_status',
                  colName: 'redirect_status',
                  name: 'redirect_status',
                  defaults: {
                      name: 'redirect_status'
                  },
                  columns: 2,
                  vertical: true,
                  margin: '0 5',
                  items: [
                    {
                        id: 'normal',
                        boxLabel: "正常",
                        inputValue: '1',
                        width: 100
                    },
                    {
                        id: 'unnormal',
                        boxLabel: "停用" + "(<span style='color:red'>停用連結，一率導回首頁</span>)",
                        checked: true,
                        inputValue: '0'
                    }
                  ]
              },
                {
                    xtype: 'displayfield',
                    fieldLabel: '群組',
                    id: 'group_name',
                    value:document.getElementById("group_name").value
                },
                {
                    xtype: 'textareafield',
                    fieldLabel: '備註',
                    id: 'redirect_note',
                    anchor: '100%'
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '建立日期',
                    id: 'sredirect_createdate',
                    hidden: row == null ? true : false
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '修改日期',
                    id: 'sredirect_updatedate',
                    hidden: row == null ? true : false
                },
                {
                    xtype: 'displayfield',
                    fieldLabel: '來源IP',
                    id: 'redirect_ipfrom',
                    hidden: row == null ? true : false
                }
        ],
        buttons: [{
            text: '保存',
            formBind: true,
            disabled: true,
            handler: function ()
            {
                var form = this.up('form').getForm();
                if (form.isValid())
                {
                    form.submit({
                        params: {
                            redirect_id: Ext.getCmp("redirect_id").getValue(),
                            redirect_name: Ext.getCmp("redirect_name").getValue(),
                            group_id: document.getElementById("group_id").value,
                            redirect_url: Ext.getCmp("redirect_url").getValue(),
                            user_group_id: Ext.getCmp("user_group_id").getValue(),
                            redirect_status: Ext.getCmp("redirect_status").getValue(),
                            redirect_note: Ext.getCmp("redirect_note").getValue()
                        },
                        success: function (form, action)
                        {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success)
                            {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                store.load();
                                editRedirectWin.close();
                            } else
                            {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function ()
                        {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    })
                }
            }
        }]
    });
    var editRedirectWin = Ext.create('Ext.window.Window', {
        title: row == null ? "連結新增" : "連結編輯",
        id: 'editRedirectWin',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        layout: 'fit',
        items: [editRedirectFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: true,
        width: 600,
        height: Ext.getCmp('editRedirectFrm').Height,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel)
             {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn)
                 {
                     if (btn == "yes")
                     {
                         Ext.getCmp('editRedirectWin').destroy();
                     }
                     else
                     {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function ()
            {
                if (row)
                {
                    if (row)
                    {
                        editRedirectFrm.getForm().loadRecord(row);//為控件賦值
                        initForm(row);
                    }
                    else
                    {
                        editRedirectFrm.getForm().reset();
                    }

                }
            }
        }
    });
    editRedirectWin.show();
    function initForm(row)
    {
        switch (row.data.redirect_status)
        {
            case 1:
                Ext.getCmp("normal").setValue(true);
                break;
            default:
                Ext.getCmp("unnormal").setValue(true);
                break;
        }
    }
}
