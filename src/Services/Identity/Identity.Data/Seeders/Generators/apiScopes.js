[
  {
    'repeat(50)': {
      DisplayName: '{{firstName()}} {{surname()}}',
      Name: function () {
        return this.DisplayName
          .match(/[A-Z]{2,}(?=[A-Z][a-z]+[0-9]*|\b)|[A-Z]?[a-z]+[0-9]*|[A-Z]|[0-9]+/g)
          .map(x => x.toLowerCase())
          .join('_');
      },
      Description: '{{lorem(5, "words")}}',
      ShowInDiscoveryDocument: '{{bool()}}',
      Enabled: function () {
        return this.ShowInDiscoveryDocument
      },
      Required: false,
      Emphasize: false,
    }
  }
]