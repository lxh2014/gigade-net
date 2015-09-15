var boolstatus = true;
editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Element/AreaPactetSave',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'packet_id',
                id: 'packet_id',
                name: 'id',
                hidden: true
            },
            {
                xtype: 'textfield',
                fieldLabel: PACKETNAME,
                name: 'name',
                id: 'packet_name',
                allowBlank: false
            },
           {
               xtype: 'combobox',
               fieldLabel: ELEMENTTYPE,
               id: 'element_type',
               name: 'element_type',
               lastQuery: '',
               allowBlank: false,
               editable: false,
               typeAhead: true,
               forceSelection: false,
               queryModel: 'local',
               store: ElementTypeStore,
               displayField: 'parameterName',
               valueField: 'ParameterCode',
               emptyText: SELECT
           },
          {
              xtype: 'numberfield',
              fieldLabel: SHOWNUMBER,
              name: 'show_number',
              id: 'show_number',
              allowBlank: false,
              value: 0,
              minValue: 0
          },
           {
               xtype: 'numberfield',
               fieldLabel: SORT,
               name: 'packet_sort',
               id: 'packet_sort',
               allowBlank: false,
               value: 0,
               minValue: 0
           },
        {
            xtype: 'textareafield',
            fieldLabel: ESCINFO,
            name: 'packet_desc',
            id: 'packet_desc',
            anchor: '100%'
        }
         //{
         //    xtype: 'textareafield',
         //    fieldLabel: '區域包ID',
         //    name: 'area_element_id',
         //    id: 'area_element_id',
         //    anchor: '100%',
         //    regex: /^\w+$/,
         //    regexText: "只能輸入字母數字下劃線組成的字符串"

         //}
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {

                var form = this.up('form').getForm();
                //if ((condition_id == "" || condition_id == "0") && (Ext.getCmp("group_name").getValue() == null || Ext.getCmp("group_name").getValue() == "")) {
                //    Ext.Msg.alert(INFORMATION, USERCONDITIONERROR);
                //    return;
                //}
                if (form.isValid()) {
                    form.submit({
                        params: {
                            id: Ext.htmlEncode(Ext.getCmp('packet_id').getValue()),
                            name: Ext.htmlEncode(Ext.getCmp('packet_name').getValue()),
                            packet_sort: Ext.htmlEncode(Ext.getCmp('packet_sort').getValue()),
                            packet_desc: Ext.htmlEncode(Ext.getCmp('packet_desc').getValue()),
                            show_number: Ext.htmlEncode(Ext.getCmp('show_number').getValue()),
                            element_type: Ext.htmlEncode(Ext.getCmp('element_type').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            if (result.success) {
                                AreaPacketStores.load();
                                editWin.close();
                            }
                            else {
                                alert(ERRORSHOW + result.success);
                            }
                        },
                        failure: function () {

                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });
                }
            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: PACKETITLE,
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 420,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        constrain: true,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSE,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row == null) {
                    editFrm.getForm().reset(); //如果是添加的話
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
}