Ext.onReady(function () {
    var BONUS_SATE = 0;
    var PalletSuggest = Ext.create('Ext.form.Panel', {
        id: 'PalletSuggest',
        url: '/WareHouse/OutPalletSuggest',
        bodyPadding: 30,
        border:false,
        width: 800,
        items: [
            {
                html: '<div class="capion">提示：匯出補貨建議報表（FIFO；副料位到主料位）</div>',
                frame: false,
                border: false

            },

          {
              xtype: 'fieldset',
              title: '匯出條件一',
              defaultType: 'textfield',
              
              layout: 'anchor',
              items: [
                    {
                        xtype: 'fieldcontainer',
                        fieldLabel: '單一品號',
                        combineErrors: true,
                        layout: 'hbox',
                        margins: '0 100 0 0',
                        defaults: {
                            width: 60
                        },
                        items: [
                        {
                            xtype: 'radiofield',
                            name: 'ra',
                            inputValue: "item_id",
                            id: "ra0",
                            checked: true,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var item_id = Ext.getCmp("item_id");
                                    var locid = Ext.getCmp("loc_id");
                                    var startIloc = Ext.getCmp("startIloc");
                                    var endIloc = Ext.getCmp("endIloc");

                                    if (newValue) {
                                        item_id.setDisabled(false);
                                        locid.setValue("");
                                        locid.setDisabled(true);
                                        startIloc.setValue("");
                                        startIloc.setDisabled(true);
                                        endIloc.setValue("");
                                        endIloc.setDisabled(true);
                                    }
                                }
                            }
                        },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           width: 400,
                           id: 'prodid',
                           layout: 'hbox',
                           items: [
                       
                      {
                          xtype: "textfield",
                          fieldLabel: '商品編號/條碼',
                          id: 'item_id',
                          labelWidth: 100,
                          disabled: false,
                          width: 250,
                          name: 'item_id'

                      }
                           ]
                       }
                        ]

                    },

              {
                  xtype: 'fieldcontainer',
                  fieldLabel: '單一主料位',
                  combineErrors: true,
                  margins: '0 100 0 0',
                  layout: 'hbox',
                  defaults: {
                      width: 60
                  },
                  items: [
                  {
                      xtype: 'radiofield',
                      name: 'ra',
                      inputValue: "freight_price",
                      id: "ra1"
                      ,
                      listeners: {
                          change: function (radio, newValue, oldValue) {
                              var item_id = Ext.getCmp("item_id");
                              var locid = Ext.getCmp("loc_id");
                              var startIloc = Ext.getCmp("startIloc");
                              var endIloc = Ext.getCmp("endIloc");


                              if (newValue) {
                                  item_id.setValue(0);
                                  
                                  item_id.setDisabled(true);
                                 
                                  locid.setDisabled(false); 
                                  startIloc.setValue("");
                                  
                                  startIloc.setDisabled(true);
                                  endIloc.setValue("");
                                  
                                  endIloc.setDisabled(true);

                              }
                          }
                      }

                  }
                  ,
                   {
                       xtype: 'fieldcontainer',
                       //combineErrors: true,
                       width: 400,
                       id: 'locid',
                       layout: 'hbox',
                       items: [
                   
                  {
                      xtype: "textfield",
                      fieldLabel: '主料位',
                      id: 'loc_id',
                      labelWidth: 100,
                      width: 250,
                      disabled: true,
                      name: 'loc_id'

                  }
                       ]
                   }
                  ]
              },
              {
                  xtype: 'fieldcontainer',
                  fieldLabel: '走道範圍',
                  combineErrors: true,
                  layout: 'hbox',
                  margins: '0 100 0 0',
                  defaults: {
                   
                      width: 60
                   
                  },
                  items: [
                  {
                      xtype: 'radiofield',
                      name: 'ra',
                      inputValue: "quantity",
                      id: "ra2"
                      ,
                      listeners: {
                          change: function (radio, newValue, oldValue) {
                              var item_id = Ext.getCmp("item_id");
                              var locid = Ext.getCmp("loc_id");
                              var startIloc = Ext.getCmp("startIloc");
                              var endIloc = Ext.getCmp("endIloc");
                              if (newValue) {
                                  item_id.setValue(0);
                                  
                                  item_id.setDisabled(true);
                                  locid.setValue("");
                                  
                                  locid.setDisabled(true);
                                  
                                  startIloc.setDisabled(false);
                                  
                                  endIloc.setDisabled(false);
                              }
                          }
                      }
                  },
                      {
                          xtype: 'fieldcontainer',
                          combineErrors: true,
                          width: 500,
                          id: 'fanwei',
                          layout: 'hbox',
                         
                          items: [
                      
                     {
                         xtype: "textfield",
                         fieldLabel: '範圍',
                         id: 'startIloc',
                         labelWidth: 100,
                         disabled: true,
                         width: 250,
                         name: 'startIloc',
                         
                         minValue: 0
                     },
                     {
                         xtype: 'displayfield',
                         margin: '0 0 0 0',
                         value: "~"
                     },
                     {
                         xtype: "textfield",
                         id: 'endIloc',
                         labelWidth: 10,
                         width: 150,
                         disabled: true,
                         name: 'endIloc',
                         
                         minValue: 0
                     }

                          ]
                      }


                  ]

              }

              ]
          }
          ,
          {
              xtype: 'fieldset',
              title: '匯出條件二',
              defaultType: 'textfield',
              layout: 'anchor',
              defaults: {
                  anchor: '100%'
              },
              items: [
                    {
                        xtype: 'fieldcontainer',
                        fieldLabel: '觸動補貨量',
                        combineErrors: true,
                        layout: 'hbox',
                        margins: '0 100 0 0',
                        defaults: {
                         
                            width: 60
                        },
                        items: [
                        {
                            xtype: 'radiofield',
                            name: 'rad',
                            inputValue: "number",
                            id: "rad0",
                            checked: true
                            ,
                            listeners: {
                                change: function (radio, newValue, oldValue) {
                                    var number = Ext.getCmp("number");



                                    if (newValue) {


                                        number.setDisabled(false);
                                        number.allowBlank = false;



                                    }
                                }
                            }
                        },
                       {
                           xtype: 'fieldcontainer',
                           combineErrors: true,
                           width: 400,

                           layout: 'hbox',
                           items: [
                              
                      {
                          xtype: "numberfield",
                          id: 'number',
                          fieldLabel:'數量',
                          labelWidth: 100,
                          disabled: false,
                          width: 250,
                          value: 0,
                          minValue:0,
                          name: 'rad',
                          allowBlank: false

                      }
                           ]
                       }
                        ]

                    },

              {
                  xtype: 'fieldcontainer',
                  fieldLabel: '自動生成',
                  combineErrors: true,

                  layout: 'hbox',
                  defaults: {
                      flex: 1,
                      hideLabel: true
                  },
                  items: [
                  {
                      xtype: 'radiofield',
                      name: 'rad',
                      inputValue: '1',
                      id: "Auto",
                      width: 20
                      ,
                      listeners: {
                          change: function (radio, newValue, oldValue) {
                              var number = Ext.getCmp("number");

                              if (newValue) {
                                  number.setValue(0);
                                  number.allowBlank = true;
                                  number.setDisabled(true);



                              }
                          }
                      }

                  }

                  ]
              }


              ]
          }
        ],
        buttons: [
            {
                formBind: true,
                disabled: true,
                text: '匯出',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {

                        window.open('/WareHouse/OutPalletSuggest?item_id=' + Ext.getCmp('item_id').getValue() + '&locid=' + Ext.getCmp('loc_id').getValue() + '&startIloc=' + Ext.getCmp('startIloc').getValue() + '&endIloc=' + Ext.getCmp('endIloc').getValue() + '&number=' + Ext.getCmp('number').getValue() + '&Auto=' + Ext.getCmp('Auto').getRawValue());
                    }
                }
            }
        ]
    })
    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        items: [PalletSuggest],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //gdIupc.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});