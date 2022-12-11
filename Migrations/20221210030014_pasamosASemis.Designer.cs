﻿// <auto-generated />
using System;
using Final.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Final.Migrations
{
    [DbContext(typeof(MiContexto))]
    [Migration("20221210030014_pasamosASemis")]
    partial class pasamosASemis
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Final.Models.CajaDeAhorro", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("cbu")
                        .HasColumnType("int");

                    b.Property<double>("saldo")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.ToTable("CajaAhorro", (string)null);
                });

            modelBuilder.Entity("Final.Models.Movimiento", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("detalle")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("fecha")
                        .HasColumnType("DateTime");

                    b.Property<int>("id_Caja")
                        .HasColumnType("int");

                    b.Property<double>("monto")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.HasIndex("id_Caja");

                    b.ToTable("Movimiento", (string)null);
                });

            modelBuilder.Entity("Final.Models.Pago", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("id_usuario")
                        .HasColumnType("int");

                    b.Property<string>("metodo")
                        .HasColumnType("varchar(50)");

                    b.Property<double>("monto")
                        .HasColumnType("float");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("pagado")
                        .HasColumnType("bit");

                    b.HasKey("id");

                    b.HasIndex("id_usuario");

                    b.ToTable("Pago", (string)null);
                });

            modelBuilder.Entity("Final.Models.PlazoFijo", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("cbu")
                        .HasColumnType("int");

                    b.Property<DateTime>("fechaFin")
                        .HasColumnType("dateTime");

                    b.Property<DateTime>("fechaIni")
                        .HasColumnType("dateTime");

                    b.Property<int>("id_titular")
                        .HasColumnType("int");

                    b.Property<double>("monto")
                        .HasColumnType("float");

                    b.Property<bool>("pagado")
                        .HasColumnType("bit");

                    b.Property<double>("tasa")
                        .HasColumnType("float");

                    b.HasKey("id");

                    b.HasIndex("id_titular");

                    b.ToTable("PlazoFijo", (string)null);
                });

            modelBuilder.Entity("Final.Models.Tarjeta", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<int>("codigoV")
                        .HasColumnType("int");

                    b.Property<double>("consumo")
                        .HasColumnType("float");

                    b.Property<int>("id_titular")
                        .HasColumnType("int");

                    b.Property<double>("limite")
                        .HasColumnType("float");

                    b.Property<int>("numero")
                        .HasColumnType("int");

                    b.HasKey("id");

                    b.HasIndex("id_titular");

                    b.ToTable("Tarjeta", (string)null);
                });

            modelBuilder.Entity("Final.Models.Usuario", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("apellido")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("bloqueado")
                        .HasColumnType("bit");

                    b.Property<int>("dni")
                        .HasColumnType("int");

                    b.Property<int>("intentosFallidos")
                        .HasColumnType("int");

                    b.Property<bool>("isAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("mail")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("id");

                    b.ToTable("Usuario", (string)null);

                    b.HasData(
                        new
                        {
                            id = 1,
                            apellido = "Muzzio",
                            bloqueado = false,
                            dni = 40009479,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "franco.muzzio@davinci.edu.ar",
                            nombre = "Franco",
                            password = "1234"
                        },
                        new
                        {
                            id = 2,
                            apellido = "Piñeiro",
                            bloqueado = false,
                            dni = 63309307,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "fiorella.piñeiro@davinci.edu.ar",
                            nombre = "Fiorella",
                            password = "1234"
                        },
                        new
                        {
                            id = 3,
                            apellido = "Markauskas",
                            bloqueado = false,
                            dni = 32677773,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "magalí.markauskas@davinci.edu.ar",
                            nombre = "Magalí",
                            password = "1234"
                        },
                        new
                        {
                            id = 4,
                            apellido = "Sassano",
                            bloqueado = false,
                            dni = 21035623,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "martín.sassano@davinci.edu.ar",
                            nombre = "Martín",
                            password = "1234"
                        },
                        new
                        {
                            id = 5,
                            apellido = "Giudice",
                            bloqueado = false,
                            dni = 23391008,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "agustín.giudice@davinci.edu.ar",
                            nombre = "Agustín",
                            password = "1234"
                        },
                        new
                        {
                            id = 6,
                            apellido = "Maubert",
                            bloqueado = false,
                            dni = 45686773,
                            intentosFallidos = 0,
                            isAdmin = true,
                            mail = "alexis.maubert@davinci.edu.ar",
                            nombre = "Alexis",
                            password = "1234"
                        },
                        new
                        {
                            id = 7,
                            apellido = "Di Marco",
                            bloqueado = false,
                            dni = 84355987,
                            intentosFallidos = 0,
                            isAdmin = false,
                            mail = "marcos.dimarco@davinci.edu.ar",
                            nombre = "Marcos",
                            password = "1234"
                        },
                        new
                        {
                            id = 8,
                            apellido = "Gutierrez",
                            bloqueado = false,
                            dni = 40563444,
                            intentosFallidos = 2,
                            isAdmin = false,
                            mail = "juliana.gutierrez@davinci.edu.ar",
                            nombre = "Juliana",
                            password = "1234"
                        },
                        new
                        {
                            id = 9,
                            apellido = "Houseman",
                            bloqueado = false,
                            dni = 30447163,
                            intentosFallidos = 0,
                            isAdmin = false,
                            mail = "ariana.houseman@davinci.edu.ar",
                            nombre = "Ariana",
                            password = "1234"
                        },
                        new
                        {
                            id = 10,
                            apellido = "Poggi",
                            bloqueado = false,
                            dni = 73026363,
                            intentosFallidos = 1,
                            isAdmin = false,
                            mail = "pedro.poggi@davinci.edu.ar",
                            nombre = "Pedro",
                            password = "1234"
                        },
                        new
                        {
                            id = 11,
                            apellido = "Ramirez",
                            bloqueado = true,
                            dni = 39440793,
                            intentosFallidos = 0,
                            isAdmin = false,
                            mail = "lazaro.ramirez@davinci.edu.ar",
                            nombre = "Lazaro",
                            password = "1234"
                        });
                });

            modelBuilder.Entity("Final.Models.UsuarioCaja", b =>
                {
                    b.Property<int>("idUsuario")
                        .HasColumnType("int");

                    b.Property<int>("idCaja")
                        .HasColumnType("int");

                    b.HasKey("idUsuario", "idCaja");

                    b.HasIndex("idCaja");

                    b.ToTable("UsuarioCaja");
                });

            modelBuilder.Entity("Final.Models.Movimiento", b =>
                {
                    b.HasOne("Final.Models.CajaDeAhorro", "caja")
                        .WithMany("movimientos")
                        .HasForeignKey("id_Caja")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("caja");
                });

            modelBuilder.Entity("Final.Models.Pago", b =>
                {
                    b.HasOne("Final.Models.Usuario", "usuario")
                        .WithMany("pagos")
                        .HasForeignKey("id_usuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("usuario");
                });

            modelBuilder.Entity("Final.Models.PlazoFijo", b =>
                {
                    b.HasOne("Final.Models.Usuario", "titular")
                        .WithMany("pf")
                        .HasForeignKey("id_titular")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("titular");
                });

            modelBuilder.Entity("Final.Models.Tarjeta", b =>
                {
                    b.HasOne("Final.Models.Usuario", "titular")
                        .WithMany("tarjetas")
                        .HasForeignKey("id_titular")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("titular");
                });

            modelBuilder.Entity("Final.Models.UsuarioCaja", b =>
                {
                    b.HasOne("Final.Models.CajaDeAhorro", "caja")
                        .WithMany("usuarioCajas")
                        .HasForeignKey("idCaja")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Final.Models.Usuario", "usuario")
                        .WithMany("usuarioCajas")
                        .HasForeignKey("idUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("caja");

                    b.Navigation("usuario");
                });

            modelBuilder.Entity("Final.Models.CajaDeAhorro", b =>
                {
                    b.Navigation("movimientos");

                    b.Navigation("usuarioCajas");
                });

            modelBuilder.Entity("Final.Models.Usuario", b =>
                {
                    b.Navigation("pagos");

                    b.Navigation("pf");

                    b.Navigation("tarjetas");

                    b.Navigation("usuarioCajas");
                });
#pragma warning restore 612, 618
        }
    }
}
