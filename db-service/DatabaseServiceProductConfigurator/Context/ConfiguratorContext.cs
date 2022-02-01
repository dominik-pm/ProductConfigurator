using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DatabaseServiceProductConfigurator.Models {
    public partial class ConfiguratorContext : DbContext {

        public ConfiguratorContext( DbContextOptions<ConfiguratorContext> options )
            : base(options) {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Configuration> Configurations { get; set; } = null!;
        public virtual DbSet<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; } = null!;
        public virtual DbSet<ConfigurationsHasLanguage> ConfigurationsHasLanguages { get; set; } = null!;
        public virtual DbSet<EDependencyType> EDependencyTypes { get; set; } = null!;
        public virtual DbSet<ELanguage> ELanguages { get; set; } = null!;
        public virtual DbSet<EOptionType> EOptionTypes { get; set; } = null!;
        public virtual DbSet<EProductCategory> EProductCategories { get; set; } = null!;
        public virtual DbSet<OptionField> OptionFields { get; set; } = null!;
        public virtual DbSet<OptionFieldHasLanguage> OptionFieldHasLanguages { get; set; } = null!;
        public virtual DbSet<OptionFieldsHasOptionField> OptionFieldsHasOptionFields { get; set; } = null!;
        public virtual DbSet<Picture> Pictures { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductHasLanguage> ProductHasLanguages { get; set; } = null!;
        public virtual DbSet<ProductsHasOptionField> ProductsHasOptionFields { get; set; } = null!;
        public virtual DbSet<ProductsHasProduct> ProductsHasProducts { get; set; } = null!;

        protected override void OnModelCreating( ModelBuilder modelBuilder ) {
            modelBuilder.UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Account>(entity => {
                entity.ToTable("account");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .HasColumnName("username");
            });

            modelBuilder.Entity<Booking>(entity => {
                entity.ToTable("bookings");

                entity.HasIndex(e => e.AccountId, "fk_BOOKINGS_ACCOUNT1_idx");

                entity.HasIndex(e => e.ConfigId, "fk_BOOKINGS_CONFIGURATIONS1_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_id");

                entity.Property(e => e.ConfigId).HasColumnName("config_id");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_BOOKINGS_ACCOUNT1");

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_BOOKINGS_CONFIGURATIONS1");
            });

            modelBuilder.Entity<Configuration>(entity => {
                entity.ToTable("configurations");

                entity.HasIndex(e => e.ProductNumber, "fk_BOOKINGS_PRODUCTS1_idx");

                entity.HasIndex(e => e.AccountId, "fk_CONFIGURATIONS_ACCOUNT1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AccountId).HasColumnName("ACCOUNT_id");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ProductNumber).HasColumnName("product_number");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("fk_CONFIGURATIONS_ACCOUNT1");

                entity.HasOne(d => d.ProductNumberNavigation)
                    .WithMany(p => p.Configurations)
                    .HasForeignKey(d => d.ProductNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_BOOKINGS_PRODUCTS1");
            });

            modelBuilder.Entity<ConfigurationHasOptionField>(entity => {
                entity.HasKey(e => new { e.ConfigId, e.OptionFieldId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("configuration_has_option_fields");

                entity.HasIndex(e => new { e.ParentConfigId, e.ParentOptionFieldId }, "fk_CONFIGURATION_has_OPTION_FIELDS_CONFIGURATION_has_OPTION_idx");

                entity.HasIndex(e => e.ConfigId, "fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_BOOKINGS1_idx");

                entity.HasIndex(e => e.OptionFieldId, "fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_OPTION_FIELD1_idx");

                entity.Property(e => e.ConfigId).HasColumnName("config_id");

                entity.Property(e => e.OptionFieldId).HasColumnName("option_field_id");

                entity.Property(e => e.ParentConfigId).HasColumnName("parent_config_id");

                entity.Property(e => e.ParentOptionFieldId).HasColumnName("parent_option_field_id");

                entity.HasOne(d => d.Config)
                    .WithMany(p => p.ConfigurationHasOptionFields)
                    .HasForeignKey(d => d.ConfigId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_BOOKINGS1");

                entity.HasOne(d => d.OptionField)
                    .WithMany(p => p.ConfigurationHasOptionFields)
                    .HasForeignKey(d => d.OptionFieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_OPTION_FIELD_has_BOOKINGS_OPTION_FIELD1");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => new { d.ParentConfigId, d.ParentOptionFieldId })
                    .HasConstraintName("fk_CONFIGURATION_has_OPTION_FIELDS_CONFIGURATION_has_OPTION_F1");

                entity.HasMany(d => d.ProductNumbers)
                    .WithMany(p => p.ConfigurationHasOptionFields)
                    .UsingEntity<Dictionary<string, object>>(
                        "ConfigurationHasSelectedOption",
                        l => l.HasOne<Product>().WithMany().HasForeignKey("ProductNumber").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_PRODUCTS1"),
                        r => r.HasOne<ConfigurationHasOptionField>().WithMany().HasForeignKey("ConfigId", "OptionFieldId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_BOOKINGS_has_OPTIO1"),
                        j => {
                            j.HasKey("ConfigId", "OptionFieldId", "ProductNumber").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

                            j.ToTable("configuration_has_selected_options");

                            j.HasIndex(new[] { "ConfigId", "OptionFieldId" }, "fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_BOOKINGS_has_OPT_idx");

                            j.HasIndex(new[] { "ProductNumber" }, "fk_BOOKINGS_has_OPTION_FIELDS_has_PRODUCTS_PRODUCTS1_idx");

                            j.IndexerProperty<int>("ConfigId").HasColumnName("config_id");

                            j.IndexerProperty<string>("OptionFieldId").HasColumnName("option_field_id");

                            j.IndexerProperty<string>("ProductNumber").HasColumnName("product_number");
                        });
            });

            modelBuilder.Entity<ConfigurationsHasLanguage>(entity => {
                entity.HasKey(e => new { e.Configuration, e.Language })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("configurations_has_language");

                entity.HasIndex(e => e.Configuration, "fk_CONFIGURATIONS_has_E_LANGUAGES_CONFIGURATIONS1_idx");

                entity.HasIndex(e => e.Language, "fk_CONFIGURATIONS_has_E_LANGUAGES_E_LANGUAGES1_idx");

                entity.Property(e => e.Configuration).HasColumnName("configuration");

                entity.Property(e => e.Language)
                    .HasMaxLength(45)
                    .HasColumnName("language");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.HasOne(d => d.ConfigurationNavigation)
                    .WithMany(p => p.ConfigurationsHasLanguages)
                    .HasForeignKey(d => d.Configuration)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CONFIGURATIONS_has_E_LANGUAGES_CONFIGURATIONS1");

                entity.HasOne(d => d.LanguageNavigation)
                    .WithMany(p => p.ConfigurationsHasLanguages)
                    .HasForeignKey(d => d.Language)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_CONFIGURATIONS_has_E_LANGUAGES_E_LANGUAGES1");
            });

            modelBuilder.Entity<EDependencyType>(entity => {
                entity.HasKey(e => e.Type)
                    .HasName("PRIMARY");

                entity.ToTable("e_dependency_types");

                entity.HasIndex(e => e.Type, "type_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<ELanguage>(entity => {
                entity.HasKey(e => e.Language)
                    .HasName("PRIMARY");

                entity.ToTable("e_languages");

                entity.HasIndex(e => e.Language, "name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Language)
                    .HasMaxLength(45)
                    .HasColumnName("language");
            });

            modelBuilder.Entity<EOptionType>(entity => {
                entity.HasKey(e => e.Type)
                    .HasName("PRIMARY");

                entity.ToTable("e_option_types");

                entity.HasIndex(e => e.Type, "TYPE_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<EProductCategory>(entity => {
                entity.HasKey(e => e.Category)
                    .HasName("PRIMARY");

                entity.ToTable("e_product_category");

                entity.HasIndex(e => e.Category, "category_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Category).HasColumnName("category");
            });

            modelBuilder.Entity<OptionField>(entity => {
                entity.ToTable("option_fields");

                entity.HasIndex(e => e.Type, "fk_OPTION_FIELD_E_OPTION_TYPES1_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Required).HasColumnName("required");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.OptionFields)
                    .HasForeignKey(d => d.Type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPTION_FIELD_E_OPTION_TYPES1");
            });

            modelBuilder.Entity<OptionFieldHasLanguage>(entity => {
                entity.HasKey(e => new { e.OptionFieldId, e.Language })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("option_field_has_language");

                entity.HasIndex(e => e.Language, "fk_ELANGUAGES_has_OPTION_FIELDS_ELANGUAGES1_idx");

                entity.HasIndex(e => e.OptionFieldId, "fk_ELANGUAGES_has_OPTION_FIELDS_OPTION_FIELDS1_idx");

                entity.Property(e => e.OptionFieldId).HasColumnName("option_field_id");

                entity.Property(e => e.Language)
                    .HasMaxLength(45)
                    .HasColumnName("language");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.HasOne(d => d.LanguageNavigation)
                    .WithMany(p => p.OptionFieldHasLanguages)
                    .HasForeignKey(d => d.Language)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ELANGUAGES_has_OPTION_FIELDS_ELANGUAGES1");

                entity.HasOne(d => d.OptionField)
                    .WithMany(p => p.OptionFieldHasLanguages)
                    .HasForeignKey(d => d.OptionFieldId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_ELANGUAGES_has_OPTION_FIELDS_OPTION_FIELDS1");
            });

            modelBuilder.Entity<OptionFieldsHasOptionField>(entity => {
                entity.HasKey(e => new { e.Base, e.OptionField })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("option_fields_has_option_fields");

                entity.HasIndex(e => e.DependencyType, "fk_OPTION_FIELDS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1_idx");

                entity.HasIndex(e => e.Base, "fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS1_idx");

                entity.HasIndex(e => e.OptionField, "fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS2_idx");

                entity.Property(e => e.Base).HasColumnName("base");

                entity.Property(e => e.OptionField).HasColumnName("option_field");

                entity.Property(e => e.DependencyType).HasColumnName("dependency_type");

                entity.HasOne(d => d.BaseNavigation)
                    .WithMany(p => p.OptionFieldsHasOptionFieldBaseNavigations)
                    .HasForeignKey(d => d.Base)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS1");

                entity.HasOne(d => d.DependencyTypeNavigation)
                    .WithMany(p => p.OptionFieldsHasOptionFields)
                    .HasForeignKey(d => d.DependencyType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPTION_FIELDS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1");

                entity.HasOne(d => d.OptionFieldNavigation)
                    .WithMany(p => p.OptionFieldsHasOptionFieldOptionFieldNavigations)
                    .HasForeignKey(d => d.OptionField)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_OPTION_FIELDS_has_OPTION_FIELDS_OPTION_FIELDS2");
            });

            modelBuilder.Entity<Picture>(entity => {
                entity.ToTable("pictures");

                entity.HasIndex(e => e.ProductNumber, "fk_Picture_PRODUCTS1_idx");

                entity.HasIndex(e => e.Id, "id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ProductNumber).HasColumnName("product_number");

                entity.Property(e => e.Url)
                    .HasColumnType("text")
                    .HasColumnName("url");

                entity.HasOne(d => d.ProductNumberNavigation)
                    .WithMany(p => p.Pictures)
                    .HasForeignKey(d => d.ProductNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Picture_PRODUCTS1");
            });

            modelBuilder.Entity<Product>(entity => {
                entity.HasKey(e => e.ProductNumber)
                    .HasName("PRIMARY");

                entity.ToTable("products");

                entity.HasIndex(e => e.Category, "fk_PRODUCTS_E_PRODUCT_CATEGORY1_idx");

                entity.HasIndex(e => e.ProductNumber, "product_number_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ProductNumber).HasColumnName("product_number");

                entity.Property(e => e.Buyable).HasColumnName("buyable");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Price)
                    .HasColumnType("float(12,2)")
                    .HasColumnName("price");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_E_PRODUCT_CATEGORY1");
            });

            modelBuilder.Entity<ProductHasLanguage>(entity => {
                entity.HasKey(e => new { e.ProductNumber, e.Language })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("product_has_language");

                entity.HasIndex(e => e.Language, "fk_PRODUCTS_has_ELANGUAGES_ELANGUAGES1_idx");

                entity.HasIndex(e => e.ProductNumber, "fk_PRODUCTS_has_ELANGUAGES_PRODUCTS1_idx");

                entity.Property(e => e.ProductNumber).HasColumnName("product_number");

                entity.Property(e => e.Language)
                    .HasMaxLength(45)
                    .HasColumnName("language");

                entity.Property(e => e.Description)
                    .HasColumnType("text")
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.HasOne(d => d.LanguageNavigation)
                    .WithMany(p => p.ProductHasLanguages)
                    .HasForeignKey(d => d.Language)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_ELANGUAGES_ELANGUAGES1");

                entity.HasOne(d => d.ProductNumberNavigation)
                    .WithMany(p => p.ProductHasLanguages)
                    .HasForeignKey(d => d.ProductNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_ELANGUAGES_PRODUCTS1");
            });

            modelBuilder.Entity<ProductsHasOptionField>(entity => {
                entity.HasKey(e => new { e.ProductNumber, e.OptionFields })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("products_has_option_fields");

                entity.HasIndex(e => e.DependencyType, "fk_PRODUCTS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1_idx");

                entity.HasIndex(e => e.OptionFields, "fk_PRODUCTS_has_OPTION_FIELDS_OPTION_FIELDS1_idx");

                entity.HasIndex(e => e.ProductNumber, "fk_PRODUCTS_has_OPTION_FIELDS_PRODUCTS1_idx");

                entity.Property(e => e.ProductNumber).HasColumnName("product_number");

                entity.Property(e => e.OptionFields).HasColumnName("option_fields");

                entity.Property(e => e.DependencyType).HasColumnName("dependency_type");

                entity.HasOne(d => d.DependencyTypeNavigation)
                    .WithMany(p => p.ProductsHasOptionFields)
                    .HasForeignKey(d => d.DependencyType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_OPTION_FIELDS_E_DEPENDENCY_TYPES1");

                entity.HasOne(d => d.OptionFieldsNavigation)
                    .WithMany(p => p.ProductsHasOptionFields)
                    .HasForeignKey(d => d.OptionFields)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_OPTION_FIELDS_OPTION_FIELDS1");

                entity.HasOne(d => d.ProductNumberNavigation)
                    .WithMany(p => p.ProductsHasOptionFields)
                    .HasForeignKey(d => d.ProductNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_OPTION_FIELDS_PRODUCTS1");
            });

            modelBuilder.Entity<ProductsHasProduct>(entity => {
                entity.HasKey(e => new { e.BaseProduct, e.OptionProduct })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("products_has_products");

                entity.HasIndex(e => e.DependencyType, "fk_PRODUCTS_has_PRODUCTS_E_DEPENDENCY_TYPE1_idx");

                entity.HasIndex(e => e.OptionProduct, "fk_PRODUCTS_has_PRODUCTS_PRODUCTS1_idx");

                entity.HasIndex(e => e.BaseProduct, "fk_PRODUCTS_has_PRODUCTS_PRODUCTS_idx");

                entity.Property(e => e.BaseProduct).HasColumnName("base_product");

                entity.Property(e => e.OptionProduct).HasColumnName("option_product");

                entity.Property(e => e.DependencyType).HasColumnName("dependency_type");

                entity.HasOne(d => d.BaseProductNavigation)
                    .WithMany(p => p.ProductsHasProductBaseProductNavigations)
                    .HasForeignKey(d => d.BaseProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_PRODUCTS_PRODUCTS");

                entity.HasOne(d => d.DependencyTypeNavigation)
                    .WithMany(p => p.ProductsHasProducts)
                    .HasForeignKey(d => d.DependencyType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_PRODUCTS_E_DEPENDENCY_TYPE1");

                entity.HasOne(d => d.OptionProductNavigation)
                    .WithMany(p => p.ProductsHasProductOptionProductNavigations)
                    .HasForeignKey(d => d.OptionProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_PRODUCTS_has_PRODUCTS_PRODUCTS1");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial( ModelBuilder modelBuilder );
    }
}
