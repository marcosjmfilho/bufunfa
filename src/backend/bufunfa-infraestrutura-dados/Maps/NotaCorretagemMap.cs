using JNogueira.Bufunfa.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JNogueira.Bufunfa.Infraestrutura.Dados.Maps
{
    public class NotaCorretagemMap : IEntityTypeConfiguration<NotaCorretagem>
    {
        public void Configure(EntityTypeBuilder<NotaCorretagem> builder)
        {
            builder.ToTable("nota_corretagem");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("IdNota");
            builder.Property(x => x.IdUsuario);
            builder.Property(x => x.DataPregao);
            builder.Property(x => x.Numero);
            builder.Property(x => x.ValorTaxaLiquidacao);
            builder.Property(x => x.ValorTaxaRegistro);
            builder.Property(x => x.ValorTaxaTermo);
            builder.Property(x => x.ValorTaxaAna);
            builder.Property(x => x.ValorEmolumentos);
            builder.Property(x => x.ValorTaxaCorretagem);
            builder.Property(x => x.ValorIss);
            builder.Property(x => x.ValorIrrf);
            builder.Property(x => x.ValorOutrasTaxas);
            builder.Property(x => x.Observacao);

            builder.HasOne(x => x.Conta)
                .WithMany()
                .HasForeignKey(x => x.IdConta);

            builder.HasMany(x => x.Lancamentos)
                .WithOne()
                .HasForeignKey(x => x.IdNota)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}