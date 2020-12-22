// BACAU
// noinspection BadExpressionStatementJS
[
  {
    "repeat(10)": {
      Firstname: '{{firstName("female")}}',
      Lastname: "{{surname()}}",
      InsertDate: function () {
        start = new Date(2020, 1, 1);
        end = new Date(2020, 5, 30);
        from = start.getTime();
        to = end.getTime();

        return new Date(from + Math.random() * (to - from));
      },
      AcceptsInformativeEmails: "{{random(true, false)}}",
      HasPicture: "{{random(true, false)}}",
      EmailConfirmed: "{{random(true, false)}}",
      PhoneNumber: "123123",
      PhoneNumberConfirmed: "{{random(true, false)}}",
      TwoFactorEnabled: "{{random(true, false)}}",
      Email: function (tags) {
        return `${this.Firstname}.${this.Lastname}@gmail.com`.toLowerCase();
      },
      Address: {
        Street: "{{street()}}",
        County: "Bacau",
        Country: "Romania",
        City:
          '{{random("Bacau", "Onesti", "Saucesti", "Comanesti", "Buhusi")}}',
      },
    },
  },
][
  // VASLUI
  // noinspection BadExpressionStatementJS
  {
    "repeat(10)": {
      Firstname: '{{firstName("female")}}',
      Lastname: "{{surname()}}",
      InsertDate: function () {
        start = new Date(2020, 1, 1);
        end = new Date(2020, 5, 30);
        from = start.getTime();
        to = end.getTime();

        return new Date(from + Math.random() * (to - from));
      },
      DisableDate: function () {
        start = new Date(2020, 6, 1);
        end = new Date(2020, 7, 30);
        from = start.getTime();
        to = end.getTime();

        return new Date(from + Math.random() * (to - from));
      },
      AcceptsInformativeEmails: "{{random(true, false)}}",
      HasPicture: "{{random(true, false)}}",
      EmailConfirmed: "{{random(true, false)}}",
      PhoneNumber: "123123",
      PhoneNumberConfirmed: "{{random(true, false)}}",
      TwoFactorEnabled: "{{random(true, false)}}",
      Email: function (tags) {
        return `${this.Firstname}.${this.Lastname}@gmail.com`.toLowerCase();
      },
      Address: {
        Street: "{{street()}}",
        County: "Vaslui",
        Country: "Romania",
        City: '{{random("Husi", "Vaslui", "Barlad", "Padureni")}}',
      },
    },
  }
];
