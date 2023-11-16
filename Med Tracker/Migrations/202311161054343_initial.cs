namespace Med_Tracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Medications",
                c => new
                    {
                        MedicationId = c.Int(nullable: false, identity: true),
                        PatientId = c.Int(nullable: false),
                        MedicationName = c.String(nullable: false, maxLength: 255),
                        MedicationType = c.String(nullable: false),
                        Dosage = c.String(nullable: false, maxLength: 50),
                        Frequency = c.String(nullable: false, maxLength: 50),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        AdminViewModel_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MedicationId)
                .ForeignKey("dbo.AdminViewModels", t => t.AdminViewModel_Id)
                .ForeignKey("dbo.Patients", t => t.PatientId, cascadeDelete: true)
                .Index(t => t.PatientId)
                .Index(t => t.AdminViewModel_Id);
            
            CreateTable(
                "dbo.Patients",
                c => new
                    {
                        PatientId = c.Int(nullable: false, identity: true),
                        NHSNumber = c.String(nullable: false, maxLength: 20),
                        Email = c.String(nullable: false, maxLength: 256),
                        PasswordHash = c.String(nullable: false, maxLength: 256),
                        Re_PasswordHash = c.String(maxLength: 256),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Confirmed = c.String(),
                        RegToken = c.String(),
                        AdminViewModel_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PatientId)
                .ForeignKey("dbo.AdminViewModels", t => t.AdminViewModel_Id)
                .Index(t => t.NHSNumber, unique: true)
                .Index(t => t.AdminViewModel_Id);
            
            CreateTable(
                "dbo.Providers",
                c => new
                    {
                        ProviderId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Email = c.String(nullable: false, maxLength: 256),
                        PhoneNumber = c.String(nullable: false),
                        PasswordHash = c.String(nullable: false, maxLength: 256),
                        Re_PasswordHash = c.String(maxLength: 256),
                        Confirmed = c.String(),
                        RegToken = c.String(),
                    })
                .PrimaryKey(t => t.ProviderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Patients", "AdminViewModel_Id", "dbo.AdminViewModels");
            DropForeignKey("dbo.Medications", "PatientId", "dbo.Patients");
            DropForeignKey("dbo.Medications", "AdminViewModel_Id", "dbo.AdminViewModels");
            DropIndex("dbo.Patients", new[] { "AdminViewModel_Id" });
            DropIndex("dbo.Patients", new[] { "NHSNumber" });
            DropIndex("dbo.Medications", new[] { "AdminViewModel_Id" });
            DropIndex("dbo.Medications", new[] { "PatientId" });
            DropTable("dbo.Providers");
            DropTable("dbo.Patients");
            DropTable("dbo.Medications");
            DropTable("dbo.AdminViewModels");
        }
    }
}
